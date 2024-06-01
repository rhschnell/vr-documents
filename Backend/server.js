const express = require('express');
const fs = require('fs');
const path = require('path');
const pdf = require('pdf-poppler');
const cors = require('cors');
const app = express();

const allowedOrigins = ['https://tychograpendaal.github.io/webxr/'];

// This is a simple CORS configuration that allows requests from the specified origins
const corsOptions = {
  origin: function (origin, callback) {
    if (allowedOrigins.indexOf(origin) !== -1 || !origin) {
      callback(null, true);
    } else {
      callback(new Error('Not allowed by CORS'));
    }
  }
};

app.use(cors(corsOptions));

// Serve the index.html file
app.use('/convert-pdf-to-image', express.raw({ type: 'application/pdf', limit: '5mb' }));

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
    scale: 3000 // Adjust resolution as needed
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

const PORT = process.env.PORT || 3000;
app.listen(PORT, () => {
  console.log(`Server is running on port ${PORT}`);
});

module.exports = app;