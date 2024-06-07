const express = require('express');
const fs = require('fs');
const path = require('path');
const pdf = require('pdf-poppler');
const { PDFDocument } = require('pdf-lib');
const { Readable } = require('stream');
const cors = require('cors');
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

app.post('/convert-pdf-to-image', (req, res) => {
  // Clear the 'temp' directory at the start
  const tempDir = path.join(__dirname, 'temp');
  fs.readdirSync(tempDir).forEach(file => {
    fs.unlinkSync(path.join(tempDir, file));
  });

  // Write the PDF to the 'temp' directory
  const tempPdfPath = path.join(tempDir, `tempPdf-${Date.now()}.pdf`);
  fs.writeFileSync(tempPdfPath, req.body);

  const outputDir = tempDir;
  const outputPath = path.basename(tempPdfPath, path.extname(tempPdfPath));

  let opts = {
    format: 'jpeg',
    out_dir: outputDir,
    out_prefix: outputPath,
    page: null, // Convert all pages
    scale: 3000 // Resolution
  };

  pdf.convert(tempPdfPath, opts)
    .then(() => {
      // Read all the image files that were created
      const imageFiles = fs.readdirSync(outputDir).filter(file => file.startsWith(outputPath));
      const imagesData = imageFiles.map(file => {
        const imagePath = path.join(outputDir, file);
        return fs.readFileSync(imagePath);
      });

      // Send back an array of images
      res.contentType('application/json');
      res.send(JSON.stringify(imagesData.map((buffer) => buffer.toString('base64'))));
    })
    .catch(err => {
      res.status(500).send(`Error converting PDF to image: ${err}`);
    });
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
      const [copiedPage] = await newPdfDoc.copyPages(pdfDoc, [pageNumber - 1]);
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

// Set up the server
const PORT = process.env.PORT || 3000;
app.listen(PORT, () => {
  console.log(`Server is running on port ${PORT}`);
});

module.exports = app;
