﻿using System.Linq;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using Umbraco.Web.Models;
using Umbraco.Web.PropertyEditors;
using Umbraco.Web;

namespace Umbraco.Tests.PropertyEditors
{
    [TestFixture]
    public class ImageCropperTest
    {
        private const string cropperJson = "{\"focalPoint\": {\"left\": 0.96,\"top\": 0.80827067669172936},\"src\": \"/media/1005/img_0671.jpg\",\"crops\": [{\"alias\":\"thumb\",\"width\": 100,\"height\": 100,\"coordinates\": {\"x1\": 0.58729977382575338,\"y1\": 0.055768992440203169,\"x2\": 0,\"y2\": 0.32457553600198386}}]}";

        private const string mediaPath = "/media/1005/img_0671.jpg";

        [Test]
        public void GetCropUrl_CropAliasTest()
        {
            var urlString = mediaPath.GetCropUrl(imageCropperValue: cropperJson, cropAlias: "Thumb", useCropDimensions: true);
            Assert.AreEqual(mediaPath + "?crop=0.58729977382575338,0.055768992440203169,0,0.32457553600198386&cropmode=percentage&width=100&height=100", urlString);
        }

        /// <summary>
        /// Test to ensure useCropDimensions is observed
        /// </summary>
        [Test]
        public void GetCropUrl_CropAliasIgnoreWidthHeightTest()
        {
            var urlString = mediaPath.GetCropUrl(imageCropperValue: cropperJson, cropAlias: "Thumb", useCropDimensions: true, width: 50, height: 50);
            Assert.AreEqual(mediaPath + "?crop=0.58729977382575338,0.055768992440203169,0,0.32457553600198386&cropmode=percentage&width=100&height=100", urlString);
        }

        [Test]
        public void GetCropUrl_WidthHeightTest()
        {
            var urlString = mediaPath.GetCropUrl(imageCropperValue: cropperJson, width: 200, height: 300);
            Assert.AreEqual(mediaPath + "?center=0.80827067669172936,0.96&mode=crop&width=200&height=300", urlString);
        }

        [Test]
        public void GetCropUrl_FocalPointTest()
        {
            var urlString = mediaPath.GetCropUrl(imageCropperValue: cropperJson, cropAlias: "thumb", preferFocalPoint: true, useCropDimensions: true);
            Assert.AreEqual(mediaPath + "?center=0.80827067669172936,0.96&mode=crop&width=100&height=100", urlString);
        }

        [Test]
        public void GetCropUrlFurtherOptionsTest()
        {
            var urlString = mediaPath.GetCropUrl(imageCropperValue: cropperJson, width: 200, height: 300, furtherOptions: "&filter=comic&roundedcorners=radius-26|bgcolor-fff");
            Assert.AreEqual(mediaPath + "?center=0.80827067669172936,0.96&mode=crop&width=200&height=300&filter=comic&roundedcorners=radius-26|bgcolor-fff", urlString);
        }

        /// <summary>
        /// Test that if a crop alias has been specified that doesn't exist the method returns null
        /// </summary>
        [Test]
        public void GetCropUrlNullTest()
        {
            var urlString = mediaPath.GetCropUrl(imageCropperValue: cropperJson, cropAlias: "Banner", useCropDimensions: true);
            Assert.AreEqual(null, urlString);
        }

        /// <summary>
        /// Test the GetCropUrl method on the ImageCropDataSet Model
        /// </summary>
        [Test]
        public void GetBaseCropUrlFromModelTest()
        {
            var cropDataSet = cropperJson.SerializeToCropDataSet();
            var urlString = cropDataSet.GetCropUrl("thumb");
            Assert.AreEqual("?crop=0.58729977382575338,0.055768992440203169,0,0.32457553600198386&cropmode=percentage&width=100&height=100", urlString);
        }

        /// <summary>
        /// Test the height ratio mode with predefined crop dimensions
        /// </summary>
        [Test]
        public void GetCropUrl_CropAliasHeightRatioModeTest()
        {
            var urlString = mediaPath.GetCropUrl(imageCropperValue: cropperJson, cropAlias: "Thumb", useCropDimensions: true, ratioMode:ImageCropRatioMode.Height);
            Assert.AreEqual(mediaPath + "?crop=0.58729977382575338,0.055768992440203169,0,0.32457553600198386&cropmode=percentage&width=100&heightratio=1", urlString);
        }

        /// <summary>
        /// Test the height ratio mode with manual width/height dimensions
        /// </summary>
        [Test]
        public void GetCropUrl_WidthHeightRatioModeTest()
        {
            var urlString = mediaPath.GetCropUrl(imageCropperValue: cropperJson, width: 300, height: 150, ratioMode:ImageCropRatioMode.Height);
            Assert.AreEqual(mediaPath + "?center=0.80827067669172936,0.96&mode=crop&width=300&heightratio=0.5", urlString);
        }

        /// <summary>
        /// Test the height ratio mode with width/height dimensions
        /// </summary>
        [Test]
        public void GetCropUrl_HeightWidthRatioModeTest()
        {
            var urlString = mediaPath.GetCropUrl(imageCropperValue: cropperJson, width: 300, height: 150, ratioMode: ImageCropRatioMode.Width);
            Assert.AreEqual(mediaPath + "?center=0.80827067669172936,0.96&mode=crop&height=150&widthratio=2", urlString);
        }

        /// <summary>
        /// Test that if Crop mode is specified as anything other than Crop the image doesn't use the crop
        /// </summary>
        [Test]
        public void GetCropUrl_SpecifiedCropModeTest()
        {
            var urlString = mediaPath.GetCropUrl(imageCropperValue: cropperJson, width: 300, height: 150, imageCropMode:ImageCropMode.Max);
            Assert.AreEqual(mediaPath + "?mode=max&width=300&height=150", urlString);
        }

        /// <summary>
        /// Test for upload property type
        /// </summary>
        [Test]
        public void GetCropUrl_UploadTypeTest()
        {
            var urlString = mediaPath.GetCropUrl(width: 100, height: 270, imageCropMode: ImageCropMode.Crop, imageCropAnchor: ImageCropAnchor.Center);
            Assert.AreEqual(mediaPath + "?mode=crop&anchor=center&width=100&height=270", urlString);
        }

        /// <summary>
        /// Test for preferFocalPoint when focal point is centered
        /// </summary>
        [Test]
        public void GetCropUrl_PreferFocalPointCenter()
        {
            var cropperJson = "{\"focalPoint\": {\"left\": 0.5,\"top\": 0.5},\"src\": \"/media/1005/img_0671.jpg\",\"crops\": [{\"alias\":\"thumb\",\"width\": 100,\"height\": 100,\"coordinates\": {\"x1\": 0.58729977382575338,\"y1\": 0.055768992440203169,\"x2\": 0,\"y2\": 0.32457553600198386}}]}";

            var urlString = mediaPath.GetCropUrl(imageCropperValue: cropperJson, width: 300, height: 150, preferFocalPoint:true);
            Assert.AreEqual(mediaPath + "?anchor=center&mode=crop&width=300&height=150", urlString);
        }

        /// <summary>
        /// Test to check if height ratio is returned for a predefined crop without coordinates and focal point in centre when a width parameter is passed
        /// </summary>
        [Test]
        public void GetCropUrl_PreDefinedCropNoCoordinatesWithWidth()
        {
            var cropperJson = "{\"focalPoint\": {\"left\": 0.5,\"top\": 0.5},\"src\": \"/media/1005/img_0671.jpg\",\"crops\": [{\"alias\": \"home\",\"width\": 270,\"height\": 161}]}";

            var urlString = mediaPath.GetCropUrl(imageCropperValue: cropperJson, cropAlias: "home", width: 200);
            Assert.AreEqual(mediaPath + "?anchor=center&mode=crop&heightratio=0.5962962962962962962962962963&width=200", urlString);
        }

        /// <summary>
        /// Test to check if height ratio is returned for a predefined crop without coordinates and focal point is custom when a width parameter is passed
        /// </summary>
        [Test]
        public void GetCropUrl_PreDefinedCropNoCoordinatesWithWidthAndFocalPoint()
        {
            var cropperJson = "{\"focalPoint\": {\"left\": 0.4275,\"top\": 0.41},\"src\": \"/media/1005/img_0671.jpg\",\"crops\": [{\"alias\": \"home\",\"width\": 270,\"height\": 161}]}";

            var urlString = mediaPath.GetCropUrl(imageCropperValue: cropperJson, cropAlias: "home", width: 200);
            Assert.AreEqual(mediaPath + "?center=0.41,0.4275&mode=crop&heightratio=0.5962962962962962962962962963&width=200", urlString);
        }

        /// <summary>
        /// Test to check if crop ratio is ignored if useCropDimensions is true
        /// </summary>
        [Test]
        public void GetCropUrl_PreDefinedCropNoCoordinatesWithWidthAndFocalPointIgnore()
        {
            var cropperJson = "{\"focalPoint\": {\"left\": 0.4275,\"top\": 0.41},\"src\": \"/media/1005/img_0671.jpg\",\"crops\": [{\"alias\": \"home\",\"width\": 270,\"height\": 161}]}";

            var urlString = mediaPath.GetCropUrl(imageCropperValue: cropperJson, cropAlias: "home", width: 200, useCropDimensions: true);
            Assert.AreEqual(mediaPath + "?center=0.41,0.4275&mode=crop&width=270&height=161", urlString);
        }

        /// <summary>
        /// Test to check if width ratio is returned for a predefined crop without coordinates and focal point in centre when a height parameter is passed
        /// </summary>
        [Test]
        public void GetCropUrl_PreDefinedCropNoCoordinatesWithHeight()
        {
            var cropperJson = "{\"focalPoint\": {\"left\": 0.5,\"top\": 0.5},\"src\": \"/media/1005/img_0671.jpg\",\"crops\": [{\"alias\": \"home\",\"width\": 270,\"height\": 161}]}";

            var urlString = mediaPath.GetCropUrl(imageCropperValue: cropperJson, cropAlias: "home", height: 200);
            Assert.AreEqual(mediaPath + "?anchor=center&mode=crop&widthratio=1.6770186335403726708074534161&height=200", urlString);
        }

        /// <summary>
        /// Test to check result when only a width parameter is passed, effectivly a resize only
        /// </summary>
        [Test]
        public void GetCropUrl_WidthOnlyParameter()
        {
            var cropperJson = "{\"focalPoint\": {\"left\": 0.5,\"top\": 0.5},\"src\": \"/media/1005/img_0671.jpg\",\"crops\": [{\"alias\": \"home\",\"width\": 270,\"height\": 161}]}";

            var urlString = mediaPath.GetCropUrl(imageCropperValue: cropperJson, width: 200);
            Assert.AreEqual(mediaPath + "?anchor=center&mode=crop&width=200", urlString);
        }

        /// <summary>
        /// Test to check result when only a height parameter is passed, effectivly a resize only
        /// </summary>
        [Test]
        public void GetCropUrl_HeightOnlyParameter()
        {
            var cropperJson = "{\"focalPoint\": {\"left\": 0.5,\"top\": 0.5},\"src\": \"/media/1005/img_0671.jpg\",\"crops\": [{\"alias\": \"home\",\"width\": 270,\"height\": 161}]}";

            var urlString = mediaPath.GetCropUrl(imageCropperValue: cropperJson, height: 200);
            Assert.AreEqual(mediaPath + "?anchor=center&mode=crop&height=200", urlString);
        }
    }
}