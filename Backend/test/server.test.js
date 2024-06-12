const chai = require('chai');
const chaiHttp = require('chai-http');
const fs = require('fs');
const path = require('path');
const app = require('../server');
const expect = chai.expect;

chai.use(chaiHttp);

// Test the image conversion endpoint
describe('PDF to Image Conversion', () => {
  const pdfPath = path.join(__dirname, 'test.pdf');

  it('should allow requests from allowed origins and convert PDF to image', (done) => {
    const pdfBuffer = fs.readFileSync(pdfPath);

    chai.request(app)
      .post('/convert-pdf-to-image')
      .set('Content-Type', 'application/pdf')
      .set('Origin', 'https://tychograpendaal.github.io/webxr/')
      .send(pdfBuffer) // Send the PDF buffer as the request body
      .end((err, res) => {
        expect(err).to.be.null;
        expect(res).to.have.status(200);
        done();
      });
  });

  it('should not allow requests from disallowed origins', (done) => {
    const pdfBuffer = fs.readFileSync(pdfPath);

    chai.request(app)
      .post('/convert-pdf-to-image')
      .set('Content-Type', 'application/pdf')
      .set('Origin', 'https://notallowed.com')
      .send(pdfBuffer) // Send the PDF buffer as the request body
      .end((err, res) => {
        // console.log(err);
        // console.log(res);
        expect(res).to.have.status(500);
        expect(res.text).to.contain('Not allowed by CORS');
        done();
      });
  });
});



// Test the extract pages endpoint
describe('Extract Pages Endpoint', () => {
  const pdfPath = path.join(__dirname, 'test.pdf');
  const pdfBuffer = fs.readFileSync(pdfPath);
  const pagesToExtract = [1, 2]; // Example pages to extract

  it('should extract specified pages from the PDF', (done) => {
    chai.request(app)
      .post('/extract-pages')
      .set('Content-Type', 'application/pdf')
      .set('Origin', 'https://tychograpendaal.github.io/webxr/')
      .set('x-pdf-pages', pagesToExtract.join(',')) // Set the pages to extract in the header
      .send(pdfBuffer) // Send the PDF buffer as the request body
      .end((err, res) => {
        expect(err).to.be.null;
        expect(res).to.have.status(200);
        done();
      });
  });

  it('should return an error if no pages are specified', (done) => {
    chai.request(app)
      .post('/extract-pages')
      .set('Content-Type', 'application/pdf')
      .set('Origin', 'https://tychograpendaal.github.io/webxr/')
      .send(pdfBuffer) // Send the PDF buffer as the request body
      .end((err, res) => {
        expect(res).to.have.status(400);
        expect(res.text).to.contain('No PDF data or pages provided.');
        done();
      });
  });

  it('should not allow requests from disallowed origins', (done) => {
    chai.request(app)
      .post('/extract-pages')
      .set('Content-Type', 'application/pdf')
      .set('Origin', 'https://notallowed.com')
      .set('x-pdf-pages', pagesToExtract.join(','))
      .send(pdfBuffer) // Send the PDF buffer as the request body
      .end((err, res) => {
        expect(res).to.have.status(500);
        expect(res.text).to.contain('Not allowed by CORS');
        done();
      });
  });
});


// Test the upload PDF endpoint
describe('Upload PDF Endpoint', () => {
  const pdfPath = path.join(__dirname, 'test.pdf'); 
  const pdfBuffer = fs.readFileSync(pdfPath);
  const pdfName = 'sample.pdf';

  it('should upload a PDF and return the file path', (done) => {
    chai.request(app)
      .post('/upload-pdf')
      .set('Content-Type', 'application/pdf')
      .set('name', pdfName)
      .send(pdfBuffer) // Send the PDF buffer as the request body
      .end((err, res) => {
        expect(err).to.be.null;
        expect(res).to.have.status(200);
        expect(res.body).to.have.property('message', 'PDF uploaded successfully');
        expect(res.body).to.have.property('filePath');
        done();
      });
  });

  it('should handle errors during PDF upload', (done) => {
    // Simulate an error condition, e.g., by sending an invalid PDF buffer
    const invalidPdfBuffer = null;
    
    chai.request(app)
      .post('/upload-pdf')
      .set('Content-Type', 'application/pdf')
      .set('name', pdfName)
      .send(invalidPdfBuffer) // Send an invalid PDF buffer as the request body
      .end((err, res) => {
        expect(res).to.have.status(400);
        expect(res.body).to.have.property('message', 'No PDF data provided');
        done();
      });
  });
});

// Test the merge PDFs endpoint
describe('Merge PDFs Endpoint', () => {
  const pdfPath1 = path.join(__dirname, 'test1.pdf'); 
  const pdfPath2 = path.join(__dirname, 'test2.pdf'); 
  const pdfBuffer1 = fs.readFileSync(pdfPath1);
  const pdfBuffer2 = fs.readFileSync(pdfPath2);

  it('should merge two PDFs and return the merged PDF', (done) => {
    // Upload the PDFs first
    Promise.all([
      chai.request(app).post('/upload-pdf').set('Content-Type', 'application/pdf').set('name', 'test1.pdf').send(pdfBuffer1),
      chai.request(app).post('/upload-pdf').set('Content-Type', 'application/pdf').set('name', 'test2.pdf').send(pdfBuffer2)
    ]).then((uploadResponses) => {
      // Perform the merge
      chai.request(app)
        .post('/merge-pdfs')
        .set('file1', 'test1.pdf')
        .set('file2', 'test2.pdf')
        .end((err, res) => {
          expect(err).to.be.null;
          expect(res).to.have.status(200);
          done();
        });
    }).catch(done);
  });
});

