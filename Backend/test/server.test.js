const chai = require('chai');
const chaiHttp = require('chai-http');
const fs = require('fs');
const path = require('path');
const app = require('../server');
const expect = chai.expect;

chai.use(chaiHttp);

describe('PDF to Image Conversion', () => {
  const pdfPath = path.join(__dirname, 'test.pdf'); // Ensure you have a test.pdf file in your test directory

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
        // Additional assertions can be made here regarding the response format
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
