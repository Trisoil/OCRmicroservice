using System;
using Leadtools.Ocr;
using System.IO;
using System.Drawing;
using Leadtools;
using Leadtools.Codecs;
using Leadtools.Drawing;
using System.Collections.Generic;
using log4net;
using System.Text.RegularExpressions;
using System.Linq;
using Leadtools.Forms.Common;
using Leadtools.Forms.Commands;
using Leadtools.Barcode;
using System.Globalization;
using Leadtools.ImageProcessing.Color;
using Leadtools.ImageProcessing.Core;
using Leadtools.ImageProcessing;
using Leadtools.Ocr.LEADEngine;
using Com.Paycasso.Divacs.Protocol;
using Com.Paycasso;

namespace OCRmicroservice
{
    public class LeadToolsOCRManager : IDisposable
    {
        #region "Properties"
        private IOcrEngine _ocrEngineOmnipage;
        private IOcrEngine _ocrEngineLead;
        private readonly ILog _log;
        private Dictionary<string, string> MapLanguages;
        private string licenseFileRelativePath, keyFileRelativePath, dir, developerKey;
        private string pathDirectoryApp;
        #endregion

        #region "Constructors"
        public LeadToolsOCRManager(ILog log)
        {
            _log = log;
            Initialization();
            StartUpOcrEngineLead();
            StartUpOcrEngineOminiPage();
        }
        #endregion

        #region "StartUpEngineLeadTtols"
        public void StartUpOcrEngineOminiPage()
        {
            StartUpOcrEngineOminiPage("en");
        }

        /// <summary>
        /// This start the OCR engine of Ominipage
        /// </summary>
        /// <param name="language"></param>
        public void StartUpOcrEngineOminiPage(String language)
        {
            try
            {
                if (System.IO.File.Exists(licenseFileRelativePath) && System.IO.File.Exists(keyFileRelativePath))
                {
                    try
                    {
                        RasterSupport.SetLicense(licenseFileRelativePath, developerKey);
                        _ocrEngineOmnipage = OcrEngineManager.CreateEngine(OcrEngineType.OmniPage, true);
                        string runtimeFolder =(@pathDirectoryApp+"LeadtoolsDependency\\Dependency\\OcrOmniPageRuntime64");
                        DirectoryInfo info = new DirectoryInfo(runtimeFolder);
                        _ocrEngineOmnipage.Startup(null, null, null, info.FullName);
                        _ocrEngineOmnipage.SpellCheckManager.SpellCheckEngine = OcrSpellCheckEngine.None;
                        _ocrEngineOmnipage.RasterCodecsInstance.Options.Load.AllPages = true;
                        //set right language
                        _ocrEngineOmnipage.LanguageManager.EnableLanguages(new string[] { language, "en" });
                        IOcrSpellCheckManager spellCheckManager = _ocrEngineOmnipage.SpellCheckManager;
                        spellCheckManager.SpellCheckEngine = OcrSpellCheckEngine.None;

                        _log.Info("Ocr engine Ominipage start status: " + _ocrEngineOmnipage.IsStarted + " for language: " + language);
                    }
                    catch (Exception ex)
                    {
                        _log.Error("Error starting OminiPage engine", ex);

                    }
                }
                else
                {
                    _log.Error("License not found ");
                    throw new Exception("License not found");
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error in StartUpOcrEngine Ominipage : ", ex);
            }
        }

        public void StartUpOcrEngineLead()
        {
            // default language is english
            StartUpOcrEngineLead("en");
        }

        /// <summary>
        /// This start the Lead OCR engine
        /// </summary>
        /// <param name="language"></param>
        public void StartUpOcrEngineLead(String language)
        {
            try
            {
                if (System.IO.File.Exists(licenseFileRelativePath) && System.IO.File.Exists(keyFileRelativePath))
                {
                    try
                    {
                        RasterSupport.SetLicense(licenseFileRelativePath, developerKey);
                        _ocrEngineLead = OcrEngineManager.CreateEngine(OcrEngineType.LEAD, true);
                        string runtimeFolder = (@pathDirectoryApp + "LeadtoolsDependency\\Dependency\\OcrLEADRuntime");
                        DirectoryInfo info = new DirectoryInfo(runtimeFolder);
                        _ocrEngineLead.Startup(null, null, null, info.FullName);
                        _ocrEngineLead.RasterCodecsInstance.Options.Load.AllPages = true;
                        //set right language
                        _ocrEngineLead.LanguageManager.EnableLanguages(new string[] { language });
                        IOcrSpellCheckManager spellCheckManager = _ocrEngineLead.SpellCheckManager;

                        if (language.Equals("en") == true)
                            spellCheckManager.SpellCheckEngine = OcrSpellCheckEngine.None;
                        else
                        {
                            spellCheckManager.SpellLanguage = language;
                            spellCheckManager.SpellCheckEngine = OcrSpellCheckEngine.Native;
                        }
                        _log.Info("Ocr engine Lead start status: " + _ocrEngineLead.IsStarted + " for language: " + language);
                    }
                    catch (Exception ex)
                    {
                        _log.Error("Error in StartUpOcrEngine Lead: ", ex);
                    }
                }
                else
                {
                    _log.Error("License LeadTools not found: ");
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error in StartUpOcrEngine Lead", ex);
            }
        }
        #endregion

        #region "reading methods"
        /// <summary>
        /// This methods reads all the image without specify any Region of the interest
        /// </summary>
        /// <param name="imageBase64String"></param>
        /// <param name="Language"></param>
        /// <returns></returns>
        public OcrResponse PerformBruteForceOcr(string imageBase64String, String Language)
        {
            var ocrResponse = new OcrResponse();
            try
            {
                if (_ocrEngineOmnipage == null || _ocrEngineOmnipage.IsStarted == false)
                {
                    StartUpOcrEngineOminiPage();
                }

                if (_ocrEngineLead == null || _ocrEngineLead.IsStarted == false)
                {
                    StartUpOcrEngineLead();
                }

                SetOCRLanguage(Language);
                var bytes = Convert.FromBase64String(imageBase64String);
                using (var mStream = new MemoryStream(bytes))
                {
                    Image imageHolder = Image.FromStream((Stream)mStream);
                    using (RasterImage rasterImage = RasterImageConverter.ConvertFromImage(imageHolder, ConvertFromImageOptions.None))
                    using (var ocrPage = _ocrEngineOmnipage.CreatePage(rasterImage, OcrImageSharingMode.None))
                    {
                        ocrPage.AutoZone(null);
                        ocrPage.Recognize(null);
                        var textOcrZones = ocrPage.Zones.Where(x => x.ZoneType == OcrZoneType.Text).ToList();

                        for (int i = 0; i < textOcrZones.Count; i++)
                        {
                            var zone = textOcrZones[i];
                            OcrRoi dataField = new OcrRoi
                            {
                                Name = zone.Name ?? "Zone" + i.ToString(),
                                X = zone.Bounds.X,
                                Y = zone.Bounds.Y,
                                W = zone.Bounds.Width,
                                H= zone.Bounds.Height
                            };
                            string value = string.Empty;

                            try
                            {
                                var zoneIndex = ocrPage.Zones.IndexOf(zone);
                                //read text
                                value = ocrPage.GetText(zoneIndex);
                            }
                            catch (Exception ex)
                            {
                                value = "";
                                Console.WriteLine(ex.Message);
                            }

                            ocrResponse.RoiValues.Add(dataField.Name, value) ;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Performing brute force ocr failed", ex);
                StartUpOcrEngineOminiPage();
            }

            return ocrResponse;
        }


        //public String CaseKindRoi(RoiKind roiKind, OcrZone ocrPage, Country country, RasterImage image)
        //{
        //    switch (roiKind)
        //    {
        //        case RoiKind.Barcode:
        //            return niente();
        //        case RoiKind.Mrz:
        //            return niente();
        //        case RoiKind.Text:
        //            return niente();
        //        case RoiKind.Unknown:
        //            return niente();
        //    }
        //    return null;
        //}

        public String ReadTextByCase() { return ""; }

        /// <summary>
        /// Perform The ROI and OCR them with the image and the propertymapper(DataField) provided by CE
        /// </summary>
        /// <param name="MachineLearningResponse"></param>
        /// <returns></returns>
        public OcrResponse PerformTargetedOcr( ref Envelope request)
        {
            OcrResponse ocrResponse = new OcrResponse();

      
            IList<OcrRoi> textDataFields = request.OcrDocument.Rois.ToList();
            String VerticalLeftOrientation = "_VL_";
            if (textDataFields != null && textDataFields.Count > 0)
            {
                try
                {
                    _log.Info("Start OCR Reader");

                    if (_ocrEngineOmnipage == null || _ocrEngineOmnipage.IsStarted == false)
                    {
                        StartUpOcrEngineOminiPage();
                    }

                    if (_ocrEngineLead == null || _ocrEngineLead.IsStarted == false)
                    {
                        StartUpOcrEngineLead();
                    }
                    

                    //set correct language before read the datafields
                    // This is the begginig the control of the language is done just once per document
                    SetOCRLanguage(request.OcrDocument.Language);

                    using (var mStream = new MemoryStream(request.OcrDocument.Image.ToByteArray()))
                    {
                        List<RectangleF> listRois = new List<RectangleF>();
                        Image imageHolder = Image.FromStream((Stream)mStream);
                        imageHolder.Save("test.jpg");
                        using (RasterImage rasterImage = RasterImageConverter.ConvertFromImage(imageHolder, ConvertFromImageOptions.None))
                        using (var ocrPageOmiPage = _ocrEngineOmnipage.CreatePage(rasterImage, OcrImageSharingMode.None))
                        using (var ocrPageLead = _ocrEngineLead.CreatePage(rasterImage, OcrImageSharingMode.None))
                        {
                            // Set the OCR zones within the rois definitions
                            foreach (var field in textDataFields)
                            {
                                var zoneOcr = new OcrZone();
                                zoneOcr.Name = field.Name;
                                zoneOcr.ZoneType = OcrZoneType.Text;

                                if (field.Name.Contains(VerticalLeftOrientation))// if it a vertical left oriented roi then change the orientation vision for OCR
                                {
                                    zoneOcr.ViewPerspective = RasterViewPerspective.RightTop;
                                    zoneOcr.Name = field.Name.Replace(VerticalLeftOrientation, "_");
                                    field.Name = field.Name.Replace(VerticalLeftOrientation, "_");
                                }

                                zoneOcr.Bounds = new LeadRect(field.X ,field.Y , field.W, field.H);
                                try
                                {
                                    ocrPageOmiPage.Zones.Add(zoneOcr);
                                    ocrPageLead.Zones.Add(zoneOcr);
                                }
                                catch(Exception ex)
                                {
                                    _log.Error("Invadid Roi region", ex);
                                }
                            }
                            ocrPageOmiPage.Recognize(null);
                            ocrPageLead.Recognize(null);

                            
                            for (int i = 0; i < ocrPageOmiPage.Zones.Count; i++)
                            {
                                var zone = ocrPageOmiPage.Zones[i];
                                LeadRect logicalRectangle = new LeadRect(zone.Bounds.X, zone.Bounds.Y, zone.Bounds.Width, zone.Bounds.Height);
                                OcrRoi ocrRoi = new OcrRoi
                                {
                                    Name = zone.Name ?? "Zone" + i.ToString(),
                                    X = zone.Bounds.X,
                                    Y = zone.Bounds.Y,
                                    W = zone.Bounds.Width,
                                    H = zone.Bounds.Height,
                                    Kind = textDataFields.Where(x => x.Name.Equals(zone.Name)).First().Kind
                                };
                     

                                string value = string.Empty;

                                try
                                {
                                    switch (ocrRoi.Kind)
                                    {
                                        // if it is barcode then call Barcode reader
                                        case RoiKind.Barcode:
                                            {
                                                value = ReadBarcode(request.OcrDocument.Image.ToByteArray(), logicalRectangle);
                                                ocrResponse.RoiValues.Add(ocrRoi.Name, value);
                                                break;
                                            }

                                        // if it is MRZ then call MRZ reader
                                        case RoiKind.Mrz:
                                            {
                                                value = TakeMRZ(request.OcrDocument.Image.ToByteArray(), logicalRectangle);
                                                ocrResponse.RoiValues.Add(ocrRoi.Name, value);
                                                break;
                                            }

                                         //  if it is text then call specific image processing and read the text
                                        case RoiKind.Text:
                                            {

                                                if (ocrRoi.Name == "ROI_DATE_OF_BIRTH_VALIDATION")
                                                {
                                                    using (RasterImage ROI_DATE_OF_BIRTH_VALIDATION = ResizeCommand(ConvertToRasterRectangle(request.OcrDocument.Image.ToByteArray(), logicalRectangle), 100, 20)) // resize the image and convert the rectangle to a RasterImage
                                                    {
                                                        var ocrPageOminipageRect = _ocrEngineOmnipage.CreatePage(ROI_DATE_OF_BIRTH_VALIDATION, OcrImageSharingMode.None);
                                                        var zoneRect = new OcrZone();
                                                        zoneRect.ZoneType = OcrZoneType.Text;
                                                        zone.Bounds = new LeadRect(0, 0, ocrPageOminipageRect.Width, ocrPageOminipageRect.Height);
                                                        ocrPageOminipageRect.Zones.Add(zone);
                                                        ocrPageOminipageRect.Recognize(null);
                                                        value = ocrPageOminipageRect.GetText(0);
                                                    }
                                                }

                                                else if (ocrRoi.Name == "ROI_ADDRESS_GROUP" && request.OcrDocument.Country.ToString() == "NZL")
                                                {
                                                    using (RasterImage ROI_ADDRESS_GROUP = OtsuThresholdCommand(ConvertToRasterRectangle(request.OcrDocument.Image.ToByteArray(), logicalRectangle))) // otsu threshold
                                                    {
                                                        var ocrPageOminipageRect = _ocrEngineOmnipage.CreatePage(ROI_ADDRESS_GROUP, OcrImageSharingMode.None);
                                                        var zoneRect = new OcrZone();
                                                        zoneRect.ZoneType = OcrZoneType.Text;
                                                        RasterCodecs codecs = new RasterCodecs();
                                                        zone.Bounds = new LeadRect(0, 0, ocrPageOminipageRect.Width, ocrPageOminipageRect.Height);
                                                        ocrPageOminipageRect.Zones.Add(zone);
                                                        ocrPageOminipageRect.Recognize(null);
                                                        value = ocrPageOminipageRect.GetText(0);
                                                    }
                                                }

                                                else if ((ocrRoi.Name == "ROI_LICENCE_TYPE") && request.OcrDocument.Country.ToString() == "AUVIC")
                                                {
                                                    using (RasterImage ROI_ADDRESS_GROUP = OtsuThresholdCommand(ConvertToRasterRectangle(request.OcrDocument.Image.ToByteArray(), logicalRectangle))) // otsu threshold
                                                    {
                                                        var ocrPageOminipageRect = _ocrEngineOmnipage.CreatePage(ROI_ADDRESS_GROUP, OcrImageSharingMode.None);
                                                        var zoneRect = new OcrZone();
                                                        zoneRect.ZoneType = OcrZoneType.Text;
                                                        RasterCodecs codecs = new RasterCodecs();
                                                        zone.Bounds = new LeadRect(0, 0, ocrPageOminipageRect.Width, ocrPageOminipageRect.Height);
                                                        ocrPageOminipageRect.Zones.Add(zone);
                                                        ocrPageOminipageRect.Recognize(null);
                                                        value = ocrPageOminipageRect.GetText(0);
                                                    }
                                                }

                                                else
                                                {
                                                    //read text
                                                    try
                                                    {
                                                        value = ocrPageOmiPage.GetText(i);
                                                        if (value == "")
                                                        {
                                                            value = ocrPageLead.GetText(i);
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        _log.Error("Errors in the final reader" + "; Exception message: " + ex.Message + "; Inner Exception: " + ex.InnerException + "; StackTrace: " + ex.StackTrace.ToString(), ex);
                                                        value = "";
                                                        StartUpOcrEngineOminiPage();
                                                        StartUpOcrEngineLead();
                                                    }
                                                }

                                                ocrResponse.RoiValues.Add(ocrRoi.Name, value);
                                                break;
                                            }
                                            
                                        // no image processing just read
                                        case RoiKind.Unknown:
                                            {
                                                value = ocrPageOmiPage.GetText(i);
                                                ocrResponse.RoiValues.Add(ocrRoi.Name, value);
                                                break;
                                            }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    value = "";
                                    _log.Error("Errors in the reading methods" + "; Exception message: " + ex.Message + "; Inner Exception: " + ex.InnerException + "; StackTrace: " + ex.StackTrace.ToString(), ex);
                                    ocrResponse = null;
                                    return ocrResponse;
                                }
                            }
                        }
                    }
                    _log.Info("End Reading");
                }              
                catch (Exception ex)
                {

                    _log.Error("Error in the initialization of OCR pages and fields" + "; Exception message: " + ex.Message + "; Inner Exception: " + ex.InnerException + "; StackTrace: " + ex.StackTrace.ToString(), ex);
                    ocrResponse = null;
                    return ocrResponse;
                }
            }

            return ocrResponse;
        }


        /// <summary>
        /// Draw the result rois in the images
        /// </summary>
        /// <param name="MachineLearningResponse"></param>
        //public void DesignRoisResult(ref Response MachineLearningResponse)
        //{
        //    try
        //    {
        //        var imageInByte = Convert.FromBase64String(MachineLearningResponse.Images.First(x => x.ImageLabel == ImageLabel.StrictCanonical).Image);
        //        using (var mStream = new MemoryStream(imageInByte))
        //        {
        //            Image imageHolder = Image.FromStream((Stream)mStream);
        //            String TransactionID = MachineLearningResponse.TransactionID;
        //            String DocumentName = MachineLearningResponse.Classification;
        //            String Country = MachineLearningResponse.Country;
        //            IList<DataField> textDataFields = MachineLearningResponse.DataFields.ToList();

        //            foreach (DataField dataField in textDataFields)
        //            {
        //                Bitmap bitmapImage = Image.FromStream(mStream) as Bitmap;
        //                Rectangle cropRect = new Rectangle((int)(dataField.X * bitmapImage.Width), (int)(dataField.Y * bitmapImage.Height), (int)(dataField.Width * bitmapImage.Width), (int)(dataField.Height * bitmapImage.Height));
        //                // Debugging mode; show the images with the rois drawed in TestImages folder
        //                using (Graphics g = Graphics.FromImage(imageHolder))
        //                {
        //                    Color customColorText = Color.FromArgb(254, Color.DarkRed);
        //                    SolidBrush shadowBrushText = new SolidBrush(customColorText);
        //                    FontFamily fontFamily = new FontFamily("Arial");
        //                    g.DrawString(dataField.FieldKey.Replace("ROI_", ""), new Font(fontFamily, 8, FontStyle.Regular, GraphicsUnit.Pixel), shadowBrushText, cropRect);
        //                    Color customColorRectangle = Color.FromArgb(50, Color.Black);
        //                    dataField.DoodadFound = textDataFields.Where(x => x.FieldKey == dataField.FieldKey).First().DoodadFound;
        //                    // show the ROIF/B Red if CE return false otherwise show it as green
        //                    if (dataField.IsImage == true && dataField.DoodadFound == false)
        //                    {
        //                        customColorRectangle = Color.FromArgb(50, Color.Red);
        //                    }
        //                    if (dataField.IsImage == true && dataField.DoodadFound == true)
        //                    {
        //                        customColorRectangle = Color.FromArgb(50, Color.Green);
        //                    }

        //                    SolidBrush shadowBrushRectangle = new SolidBrush(customColorRectangle);
        //                    g.FillRectangle(shadowBrushRectangle, cropRect);
        //                }
        //            }
        //            DirectoryInfo TestImages = Directory.CreateDirectory(pathDirectoryApp + "\\TestImages");

        //            //one transactin can have 3 different images ,then them need to be saved with different name
        //            if (File.Exists(pathDirectoryApp+"TestImages\\" + TransactionID.ToString() + "_" + DocumentName + ".png"))
        //            {
        //                if (File.Exists(pathDirectoryApp + "TestImages\\" + TransactionID.ToString() + "_2" + DocumentName + ".png"))
        //                {
        //                    imageHolder.Save(pathDirectoryApp + "TestImages\\" + TransactionID.ToString() + "_3" + DocumentName + ".png");
        //                }
        //                else
        //                    imageHolder.Save(pathDirectoryApp + "TestImages\\" + TransactionID.ToString() + "_2" + DocumentName + ".png");
        //            }
        //            else
        //                imageHolder.Save(pathDirectoryApp + "TestImages\\" + TransactionID.ToString() + "_" + DocumentName + ".png");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _log.Error("Error drawing the images: ", ex);
        //    }
        //}

        /// <summary>
        /// This function is used to read the barcode.
        /// </summary>
        /// <param name="imageBase64String"></param>
        /// <returns>String  converted from Barcode code, if it is not read then return ""</returns>
        public String ReadBarcode(byte[] bytes, LeadRect logicalRectangle)
        {
            string Result = "";
            RasterCodecs rasterCodecs;
            Leadtools.RasterImage rasterimage;
            Leadtools.RasterImage rasterimageCut;
            Image image;
            Bitmap bitmapImage;

            try
            {
                rasterCodecs = new RasterCodecs();

                //set raster image
                MemoryStream ms = new MemoryStream(bytes);
                bitmapImage = Image.FromStream(ms) as Bitmap;

                // crop image
                Rectangle cropRect = new Rectangle((int)(logicalRectangle.X), (int)(logicalRectangle.Y), (int)(logicalRectangle.Width), (int)(logicalRectangle.Height));
                Image imageCropped = bitmapImage.Clone(cropRect, bitmapImage.PixelFormat);

                //imageCropped.Save(TransactionID + "Barcode" + ".png");
                rasterimageCut = RasterImageConverter.ConvertFromImage(imageCropped, ConvertFromImageOptions.None);
                image = Image.FromStream(ms);
                rasterimage = RasterImageConverter.ConvertFromImage(image, ConvertFromImageOptions.None);

                //Initialize Barcode fro big barcode    
                BarcodeEngine barcodeEngine = new BarcodeEngine();
                Leadtools.Barcode.BarcodeReader reader = barcodeEngine.Reader;
                reader.ErrorMode = BarcodeReaderErrorMode.IgnoreAll;
                reader.ImageType = BarcodeImageType.Unknown;
                BarcodeEngine barcodeEngineSmallSize = new BarcodeEngine(); // reader for small barcode
                Leadtools.Barcode.BarcodeReader readersmallbarcode = barcodeEngineSmallSize.Reader;
                readersmallbarcode.ErrorMode = BarcodeReaderErrorMode.IgnoreAll;
                readersmallbarcode.ImageType = BarcodeImageType.Unknown;
                OneDBarcodeReadOptions oneDReadOptions = barcodeEngineSmallSize.Reader.GetDefaultOptions(BarcodeSymbology.UPCA) as OneDBarcodeReadOptions;
                oneDReadOptions.ResizeSmall1D = true;

                BarcodeSymbology[] Symb = barcodeEngine.Reader.GetAvailableSymbologies();
                ///First try is the region of the rectangle, if it doesn't work try with all area of the image
                BarcodeData[] barcodes = reader.ReadBarcodes(rasterimage, logicalRectangle, 0, Symb, null);
                if (barcodes.Length < 1)
                    barcodes = barcodeEngineSmallSize.Reader.ReadBarcodes(rasterimage, logicalRectangle, 0, Symb, null);

                if (barcodes.Length > 0)
                {
                    Result = barcodes[0].Value;

                }
                // try with all the image
                else
                {
                    barcodes = reader.ReadBarcodes(rasterimage, LeadRect.Empty, 0, Symb, null);
                    if (barcodes.Length < 1)
                        barcodes = barcodeEngineSmallSize.Reader.ReadBarcodes(rasterimage, LeadRect.Empty, 0, Symb, null);

                    if (barcodes.Length > 0)
                    {
                        Result = barcodes[0].Value;
                    }
                }

                // Try image processing Binarize if not barcode is read
                if (Result.Length < 1)
                {
                    // apply binarization
                    rasterimage = AutoBinarizeCommand(rasterimage);
                    rasterimageCut = AutoBinarizeCommand(rasterimageCut);
                    // try rectangle
                    barcodes = reader.ReadBarcodes(rasterimage, logicalRectangle, 0, Symb, null);
                    if (barcodes.Length < 1)
                        barcodes = barcodeEngineSmallSize.Reader.ReadBarcodes(rasterimage, logicalRectangle, 0, Symb, null);

                    if (barcodes.Length > 0)
                    {
                        Result = barcodes[0].Value;
                    }
                    else
                    {
                        // try all the image
                        barcodes = reader.ReadBarcodes(rasterimage, LeadRect.Empty, 0, Symb, null);
                        if (barcodes.Length < 1)
                            barcodes = barcodeEngineSmallSize.Reader.ReadBarcodes(rasterimage, LeadRect.Empty, 0, Symb, null);

                        if (barcodes.Length > 0)
                        {
                            Result = barcodes[0].Value;
                        }
                        else // try just with the manually cut image
                        {
                            barcodes = reader.ReadBarcodes(rasterimageCut, LeadRect.Empty, 0, Symb, null);
                            if (barcodes.Length < 1)
                                barcodes = barcodeEngineSmallSize.Reader.ReadBarcodes(rasterimageCut, LeadRect.Empty, 0, Symb, null);

                            if (barcodes.Length > 0)
                            {
                                Result = barcodes[0].Value;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.StackTrace.ToString();
                _log.Error("Error reading Barcode: ", ex);
                StartUpOcrEngineOminiPage();
                StartUpOcrEngineLead();
            }
            return Result;
        }

        /// <summary>
        /// Convert image from image byte[] to MRZ information (line separeted by \n)
        /// </summary>
        /// <param name="imageBase64String"></param>
        public String TakeMRZ(byte[] bytes, LeadRect logicalRectangle)
        {
            String Result = "";
            MRTDReader _reader;
            Leadtools.RasterImage rasterimageCropped, fullRasterImage;
            Bitmap bitmapImage;
            try
            {
                _reader = new MRTDReader();
                _reader.OcrEngine = _ocrEngineLead;
                MemoryStream ms = new MemoryStream(bytes);
                bitmapImage = Image.FromStream(ms) as Bitmap;
                // crop image
                Rectangle cropRect = new Rectangle((int)(logicalRectangle.X), (int)(logicalRectangle.Y), (int)(logicalRectangle.Width), (int)(logicalRectangle.Height));
                Image imageCropped = bitmapImage.Clone(cropRect, bitmapImage.PixelFormat);
                // imageCropped.Save("test.png");
                rasterimageCropped = RasterImageConverter.ConvertFromImage(imageCropped, ConvertFromImageOptions.None);
                //Read MRZ processing from leadtools
                _reader.ProcessImage(rasterimageCropped);
                //Put the MRZ information into Result
                //if it is not read well the crop image, try with all the image
                if (_reader.Errors != MRTDErrors.NoError)
                {
                    fullRasterImage = RasterImageConverter.ConvertFromImage(bitmapImage, ConvertFromImageOptions.None);
                    _reader.ProcessImage(fullRasterImage);
                }
                //If also don't read with the full image try just with the cropImage and return a string with errors
                if (_reader.Errors != MRTDErrors.NoError)
                {
                    _reader.ProcessImage(rasterimageCropped);
                }
                for (int i = 0; i < _reader.Lines.Length; i++)
                {
                    Result += _reader.Lines[i];
                    Result += "\n";
                }
            }
            catch (Exception ex)
            {
                ex.StackTrace.ToString();
                _log.Error("Error reading MRZ: ", ex);
                StartUpOcrEngineOminiPage();
            }
            return Result;
        }



        /// <summary>
        /// Convert the image within the rectangle to a new Raster image with just the pixel of the rectangle Area
        /// </summary>
        /// <param name="imageBase64String"></param>
        /// <param name="logicalRectangle"></param>
        /// <returns></returns>
        public RasterImage ConvertToRasterRectangle( byte[] bytes, LeadRect logicalRectangle)
        {
            Leadtools.RasterImage rasterimageCropped = null;
            Bitmap bitmapImage;
            try
            {
                MemoryStream ms = new MemoryStream(bytes);
                bitmapImage = Image.FromStream(ms) as Bitmap;
                // crop image
                Rectangle cropRect = new Rectangle((int)(logicalRectangle.X), (int)(logicalRectangle.Y), (int)(logicalRectangle.Width), (int)(logicalRectangle.Height));
                Image imageCropped = bitmapImage.Clone(cropRect, bitmapImage.PixelFormat);
                rasterimageCropped = RasterImageConverter.ConvertFromImage(imageCropped, ConvertFromImageOptions.None);
                return rasterimageCropped;
            }
            catch (Exception ex)
            {
                _log.Error("Error to convert the image to Raster Image Rectangle: ", ex);
            }
            return rasterimageCropped;
        }

        #endregion

        #region "Comparison"

        public void CorrelationCommandExample()
        {
            // Load an image
            RasterCodecs codecs = new RasterCodecs();
            codecs.ThrowExceptionsOnInvalidImages = true;

            RasterImage image = codecs.Load(Path.Combine(LEAD_VARS.ImagesDir, "Master.jpg"));

            // Prepare the command
            RasterImage DstImage = image.Clone();
            CorrelationCommand command = new CorrelationCommand();
            command.CorrelationImage = DstImage;
            command.Threshold = 70;
            command.XStep = 1;
            command.YStep = 1;
            command.Points = new LeadPoint[90];
            //Apply the correlation filter.
            command.Run(image);

        }

        static class LEAD_VARS
        {
            public const string ImagesDir = @"C:\Users\Public\Documents\LEADTOOLS Images";
        }

        #endregion

        #region "Methods"

        /// <summary>
        /// Bicubic interpolation
        /// </summary>
        /// <param name="srcImage"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public RasterImage ResizeCommand(RasterImage srcImage, int width, int height)
        {
            RasterCodecs codecs = new RasterCodecs();

            // Create the destination image 
            RasterImage destImage = new RasterImage(
               RasterMemoryFlags.Conventional,
               width,
               height,
               srcImage.BitsPerPixel,
               srcImage.Order,
               srcImage.ViewPerspective,
               srcImage.GetPalette(),
               IntPtr.Zero,
               0);

            // Resize the source image into the destination image 
            ResizeCommand command = new ResizeCommand();
            command.DestinationImage = destImage;
            command.Flags = RasterSizeFlags.Bicubic;
            command.Run(srcImage);
            // Clean Up 
            srcImage.Dispose();
            codecs.Dispose();
            return destImage;
        }

        /// <summary>
        /// 
        // This is a MAP of the languages currently supported by default without others dictionary, if in the future we need other
        //languages then we need to install them separately
        /// </summary>
        private void InitializeLanguages()
        {
            MapLanguages = new Dictionary<string, string>();
            MapLanguages.Add("Unknown", "en");
            MapLanguages.Add("English", "en");
            MapLanguages.Add("Spanish", "es");
            MapLanguages.Add("Italian", "it");
            MapLanguages.Add("French", "fr");
            MapLanguages.Add("Icelandic", "is");
            MapLanguages.Add("Catalan", "ca");
            MapLanguages.Add("Basque", "eu");
            MapLanguages.Add("Faroese", "fo");
            MapLanguages.Add("Afrikaans", "af");
            MapLanguages.Add("Indonesian", "id");
            MapLanguages.Add("German", "de");
            MapLanguages.Add("Lithuanian", "lt");
            MapLanguages.Add("Bulgarian", "lt");
            MapLanguages.Add("Belarusian", "be");
            MapLanguages.Add("Dutch", "nl");
            MapLanguages.Add("Estonian", "et");
            MapLanguages.Add("Finnish", "fi");
            MapLanguages.Add("Galician", "gl");
            MapLanguages.Add("Greek", "el");
            MapLanguages.Add("Hungarian", "hu");
            MapLanguages.Add("Japanese", "ja");
            MapLanguages.Add("Korean", "ko");
            MapLanguages.Add("Latvian", "lv");
            MapLanguages.Add("Macedonian", "mk");
            MapLanguages.Add("Malay", "ms");
            MapLanguages.Add("Maltese", "mt");
            MapLanguages.Add("Norwegian", "no");
            MapLanguages.Add("Polish", "pl");
            MapLanguages.Add("Portuguese", "pt");
            MapLanguages.Add("Chinese Simplified", "zh-Hans");
            MapLanguages.Add("Chinese Traditional", "zh-Hant");
            MapLanguages.Add("Romanian", "ro");
            MapLanguages.Add("Russian", "ru");
            MapLanguages.Add("Serbian", "sr");
            MapLanguages.Add("Slovak", "sk");
            MapLanguages.Add("Slovenian", "sl");
            MapLanguages.Add("Swahili", "sw");
            MapLanguages.Add("Swedish", "sv");
            MapLanguages.Add("Telugu", "te");
            MapLanguages.Add("Thai", "th");
            MapLanguages.Add("Turkish", "tr");
            MapLanguages.Add("Ukrainian", "uk");
            MapLanguages.Add("Vietnamese", "vi");
        }

        /// <summary>
        /// Initialization of the support dependencies
        /// </summary>
        public void Initialization()
        {
            try
            {
                pathDirectoryApp = System.AppDomain.CurrentDomain.BaseDirectory;
                pathDirectoryApp += "\\";
                // set dependencies
                licenseFileRelativePath = (@pathDirectoryApp + "LeadtoolsDependency\\Dependency\\LEADTOOLS.lic");
                keyFileRelativePath = (@pathDirectoryApp + "LeadtoolsDependency\\Dependency\\LEADTOOLS.lic.key");
                dir = pathDirectoryApp;// System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                developerKey = System.IO.File.ReadAllText(keyFileRelativePath);
                //set languages
                InitializeLanguages();
               
            }
            catch (Exception ex)
            {
                _log.Error("Error in Initialization LeadToolsOCRManager : ", ex);
            }
        }

        /// <summary>
        /// Set correct OCR Language
        /// </summary>
        /// <param name="TwoLetterISOLanguage"></param>
        public void SetOCRLanguage(String Language)
        {
            try
            {
                String TwoLetterISOLanguage = MapLanguages[Language];
                // if the language is different than the current one then change it!
                if (_ocrEngineOmnipage.LanguageManager.GetEnabledLanguages()[0].Equals(TwoLetterISOLanguage) == false)
                {
                    CultureInfo currentCulture = CultureInfo.CurrentCulture;
                    string name = currentCulture.TwoLetterISOLanguageName;
                    bool supported = _ocrEngineOmnipage.LanguageManager.IsLanguageSupported(name);

                    if (!supported)
                    {
                        name = currentCulture.Name;
                        supported = _ocrEngineOmnipage.LanguageManager.IsLanguageSupported(name);
                    }

                    if (supported)
                    {
                        Dispose();
                        // if it is english we use a different configuration than for the native languages, because the english language is very various
                        if (TwoLetterISOLanguage.Equals("en") == true)
                        {
                            StartUpOcrEngineOminiPage();
                            StartUpOcrEngineLead();
                        }
                        else
                        {
                            StartUpOcrEngineOminiPage(TwoLetterISOLanguage);
                            StartUpOcrEngineLead(TwoLetterISOLanguage);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error in swap languange OCR to: " + Language + ". ", ex);
                StartUpOcrEngineOminiPage();
                StartUpOcrEngineLead();
            }
        }

        public List<OcrZone> GetZones(Image bitmap)
        {
            if (bitmap.Height > 30)
            {
                OcrResponse ocrResponse = new OcrResponse();
                RasterImage rasterImage = RasterImageConverter.ConvertFromImage(bitmap, ConvertFromImageOptions.None);
                using (var _ocrPage = _ocrEngineOmnipage.CreatePage(rasterImage, OcrImageSharingMode.AutoDispose))
                {
                    _ocrPage.AutoZone(null);
                    return _ocrPage.Zones.Where(x => x.ZoneType == OcrZoneType.Text).ToList();
                }
            }

            return new List<OcrZone>();
        }
        public void Dispose()
        {
            _ocrEngineOmnipage?.Dispose();
            _ocrEngineLead?.Dispose();
        }

        /// <summary>
        /// This is a new function that is used to remove the background of the image; it is not used yet
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        public Image RemoveBackGround(MemoryStream ms)
        {
            Bitmap ImageBitmap = Image.FromStream(ms) as Bitmap;
            try
            {
                // byte[] bytes = Convert.FromBase64String(imageBase64String);
                //MemoryStream ms = new MemoryStream(bytes);
                Color TempPixelColor;

                //loop the image
                for (int i = 0; i < ImageBitmap.Width; i++)
                {
                    for (int j = 0; j < ImageBitmap.Height; j++)
                    {
                        TempPixelColor = ImageBitmap.GetPixel(i, j);

                        if ((TempPixelColor.G >= TempPixelColor.R - 40 && TempPixelColor.G <= TempPixelColor.R + 40) && (TempPixelColor.G >= TempPixelColor.B - 40 && TempPixelColor.G <= TempPixelColor.B + 40) && TempPixelColor.G < 155)
                        {
                            // that s ok
                        }
                        else
                        {
                            // Remove Backgroung
                            ImageBitmap.SetPixel(i, j, Color.White);
                        }
                    }
                }
                //  ImageBitmap.Save("ImageWithoutBackground.png");
            }
            catch (Exception ex)
            {
                ex.StackTrace.ToString();
            }
            return (ImageBitmap as Image);
            // return ImageBitmap;
        }


        /// <summary>
        /// Auto Binarize convert an image to gray scale
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public RasterImage AutoBinarizeCommand(RasterImage image)
        {
            try
            {
                AutoBinarizeCommand command = new AutoBinarizeCommand();
                command.Run(image);
            }
            catch (Exception ex)
            {
                _log.Error("Error in the Auto Binary Command: ", ex);
            }

            return image;
        }

        /// <summary>
        /// Convert image to gray scale
        /// </summary>
        /// <param name="image">Image</param>
        /// <returns>RasterImage</returns>
        public RasterImage DynamicBinaryCommand(RasterImage image)
        {
            try
            {
                // Prepare the command 
                DynamicBinaryCommand command = new DynamicBinaryCommand();
                command.Dimension = 8;
                command.LocalContrast = 16;
                // convert it into a black and white image without changing its bits per pixel. 
                command.Run(image);
            }
            catch (Exception ex)
            {
                _log.Error("Error in the Dynamic Binary Command: ", ex);
            }

            return image;
        }

        /// <summary>
        /// Removes speckles from a bitmap, particularly scanned images such as Faxes.
        /// Remove the speckles in order to allow more accurate OCR and barcode detection.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public RasterImage DespeckleCommand(RasterImage image)
        {
            try
            {
                // Prepare the command 
                DespeckleCommand command = new DespeckleCommand();
                //Remove speckles from the image. 
                command.Run(image);
            }
            catch (Exception ex)
            {
                _log.Error("Error in the Despeckle Command: ", ex);
            }

            return image;
        }


        /// <summary>
        /// removes the dots from an image. it requires a 1 bit image
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public RasterImage DotRemoveCommand(RasterImage image)
        {
            try
            {
                // Prepare the command 
                DotRemoveCommand command = new DotRemoveCommand();
                command.DotRemove += new EventHandler<DotRemoveCommandEventArgs>(DotRemoveEvent_S1);
                command.Flags = DotRemoveCommandFlags.UseSize;
                command.MaximumDotHeight = 10;
                command.MaximumDotWidth = 10;
                command.MinimumDotHeight = 1;
                command.MinimumDotWidth = 1;
                command.Run(image);

            }
            catch (Exception ex)
            {
                _log.Error("Error in the Dot Remove Command: ", ex);
            }
            return image;
        }


        private void DotRemoveEvent_S1(object sender, DotRemoveCommandEventArgs e)
        {
            //Do not remove the speck if it contains any white pixels 
            if (e.WhiteCount > 0)
            {
                e.Status = RemoveStatus.NoRemove;
            }
            else
            {
                e.Status = RemoveStatus.Remove;
            }
        }


        /// <summary>
        /// Remove lines. It requires an image 1 bit
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public RasterImage LineRemoveCommand(RasterImage image)
        {
            try
            {
                // Prepare the command 
                LineRemoveCommand command = new LineRemoveCommand();
                command.LineRemove += new EventHandler<LineRemoveCommandEventArgs>(LineRemoveEvent_S1);
                command.Type = LineRemoveCommandType.Horizontal;
                command.Flags = LineRemoveCommandFlags.UseGap;
                command.GapLength = 2;
                command.MaximumLineWidth = 5;
                command.MinimumLineLength = 200;
                command.MaximumWallPercent = 10;
                command.Wall = 7;
                command.Run(image);
            }
            catch (Exception ex)
            {
                _log.Error("Error in the Line Remove Command: ", ex);
            }

            return image;
        }

        private void LineRemoveEvent_S1(object sender, LineRemoveCommandEventArgs e)
        {
            e.Status = RemoveStatus.Remove;
        }

        /// <summary>
        /// Otsu Threshold
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public RasterImage OtsuThresholdCommand(RasterImage image)
        {
            try
            {
                //Prepare the command 
                OtsuThresholdCommand command = new OtsuThresholdCommand();
                //Apply  
                command.Clusters = 4;
                command.Run(image);
            }
            catch (Exception ex)
            {
                _log.Error("Error in Otsu Threshold Command: ", ex);
            }
            return image;
        }
        #endregion
    }
}
