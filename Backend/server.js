const express = require('express');
const fs = require('fs');
const path = require('path');
const { Poppler } = require("node-poppler");
const { PDFDocument } = require('pdf-lib');
const { Readable } = require('stream');
const cors = require('cors');
var http = require('http');
const app = express();


// Enable all CORS requests
// app.use(cors());

const allowedOrigin = 'https://tychograpendaal.github.io';

// CORS configuration
const corsOptions = {
  origin: function (origin, callback) {
    if (!origin || origin.startsWith(allowedOrigin)) {
      callback(null, true);
    } else {
      console.log(`Origin ${origin} not allowed by CORS`);
      callback(new Error('Not allowed by CORS'));
    }
  }
};

app.use(cors(corsOptions));
app.use(express.json());
app.use(express.urlencoded({ extended: true }));

// Middleware for handling raw PDF data
app.use('/convert-pdf-to-image', express.raw({ type: 'application/pdf', limit: '5mb' }));
app.use('/extract-pages', express.raw({ type: 'application/pdf', limit: '5mb' }));
app.use('/upload-pdf', express.raw({ type: 'application/pdf', limit: '5mb' }));


// Endpoint that receives a PDF file and writes it to the uploads folder
app.post('/upload-pdf', (req, res) => {
  try {
    console.log("upload-pdf")

    // Retrieve the name of the file from the headers
    const name = req.headers['name'];
    console.log(name);
    console.log(req.body);

    // Ensure that the request contains actual PDF data
    if (!req.body || !req.body.length) {
      return res.status(400).send({ message: 'No PDF data provided' });
    }


    // Write the PDF file to the uploads folder
    const filePath = path.join(__dirname, 'uploads', name);
    fs.writeFileSync(filePath, req.body);

    // Send a response to the client
    res.status(200).send({ message: 'PDF uploaded successfully', filePath });
  } catch (error) {
    console.error(error);
    res.status(500).send({ message: 'Error uploading PDF' });
  }
});


// Endpoint for merging two PDFs
app.post('/merge-pdfs', async (req, res) => {
  try {
    // retrieve the names of the files
    const file1 = req.headers['file1'];
    const file2 = req.headers['file2'];
    
    // Read the PDF files from the uploads folder
    const file1bytes = fs.readFileSync(path.join(__dirname, 'uploads', file1));
    const file2bytes = fs.readFileSync(path.join(__dirname, 'uploads', file2));

    // delete the files from the uploads folder
    fs.unlinkSync(path.join(__dirname, 'uploads', file1));
    fs.unlinkSync(path.join(__dirname, 'uploads', file2));

    // Create new PDF documents
    const [pdfDoc1, pdfDoc2] = await Promise.all([
      PDFDocument.load(file1bytes),
      PDFDocument.load(file2bytes)
    ]);

    console.log("made it");

    // Create a new PDF document
    const mergedPdfDoc = await PDFDocument.create();

    // Copy pages from the first PDF to the new document
    for (const pageNumber of pdfDoc1.getPageIndices()) {
      const [copiedPage] = await mergedPdfDoc.copyPages(pdfDoc1, [pageNumber]);
      mergedPdfDoc.addPage(copiedPage);
    }

    // // Copy pages from the second PDF to the new document
    for (const pageNumber of pdfDoc2.getPageIndices()) {
      const [copiedPage] = await mergedPdfDoc.copyPages(pdfDoc2, [pageNumber]);
      mergedPdfDoc.addPage(copiedPage);
    }

    // Serialize the merged PDF document to bytes
    const mergedPdfBytes = await mergedPdfDoc.save();

    // Create a stream from the new PDF bytes
    const pdfStream = new Readable();
    pdfStream.push(mergedPdfBytes);
    pdfStream.push(null); // Indicate the end of the stream
    
    res.contentType('application/pdf');
    pdfStream.pipe(res); // Pipe the stream to the response

  } catch (error) {
    console.error(error);
    res.status(500).send({ message: 'Error merging PDFs' });
  }
});

// Endpoint for converting a PDF to images
app.post('/convert-pdf-to-image', async (req, res) => {
  console.log("convert-pdf-to-image");
  const tempDir = path.join(__dirname, 'temp');

  // Write the PDF to the 'temp' directory
  const currentDate = Date.now();
  const tempPdfPath = path.join(tempDir, `tempPDF-${currentDate}.pdf`);
  fs.writeFileSync(tempPdfPath, req.body);

  // Set the options for the conversion
  const poppler = new Poppler();
  const options = {
    pngFile: true,
    resolutionXYAxis: 300,
  };
  
  const outputFile = path.join(tempDir, `tempPNG-${currentDate}.png`);

  await poppler.pdfToCairo(tempPdfPath, outputFile, options);

  // Read all the image files that were created
  const imageFiles = fs.readdirSync(tempDir).filter(file => file.startsWith(`tempPNG-${currentDate}`));
  const imagesData = imageFiles.map(file => {
    const imagePath = path.join(tempDir, file);
    return fs.readFileSync(imagePath);
  });

  // Send back an array of images
  res.contentType('application/json');
  await res.send(JSON.stringify(imagesData.map((buffer) => buffer.toString('base64'))));

  // Clear all the files in the temp directory with the current date
  const files = fs.readdirSync(tempDir);
  for (const file of files) {
    if (
      file.startsWith(`tempPDF-${currentDate}`) || 
      file.startsWith(`tempPNG-${currentDate}`)
    ) {
      fs.unlinkSync(path.join(tempDir, file));
    }
  }
});

// Endpoint for extracting pages from a PDF
app.post('/extract-pages', async (req, res) => {
  console.log(req.body);
  console.log(req.headers['x-pdf-pages']);
  // Ensure that the request contains raw PDF data and pages parameter
  if (!req.body || !req.body.length || !req.headers['x-pdf-pages']) {
    return res.status(400).send('No PDF data or pages provided.');
  }

  try {
    const pagesToExtract = req.headers['x-pdf-pages'].split(',').map(Number);
    const existingPdfBytes = req.body;
    
    const pdfDoc = await PDFDocument.load(existingPdfBytes);
    const newPdfDoc = await PDFDocument.create();
    
    for (const pageNumber of pagesToExtract) {
      const [copiedPage] = await newPdfDoc.copyPages(pdfDoc, [pageNumber]);
      newPdfDoc.addPage(copiedPage);
    }
    
    const newPdfBytes = await newPdfDoc.save();
    
    // Create a stream from the new PDF bytes
    const pdfStream = new Readable();
    pdfStream.push(newPdfBytes);
    pdfStream.push(null); // Indicate the end of the stream
    
    res.contentType('application/pdf');
    pdfStream.pipe(res); // Pipe the stream to the response
  } catch (error) {
    console.log(error);
    res.status(500).send('Error processing PDF: ' + error.message);
  }
});

// get method
app.get('/', (req, res) => {
  res.send('Hello from App Engine!');
});

// Create an HTTP service.
// http.createServer(app).listen(8080, "0.0.0.0");

// Set up the server
const PORT = process.env.PORT || 8080;
app.listen(PORT, () => {
  console.log(`Server is running on port ${PORT}`);
});

module.exports = app;
