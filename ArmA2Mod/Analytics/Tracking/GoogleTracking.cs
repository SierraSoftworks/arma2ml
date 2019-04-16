/*
 * Facebook Google Analytics Tracker
 * Copyright 2010 Doug Rathbone
 * http://www.diaryofaninja.com
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;
using SierraLib.Analytics.Common.Data;
using SierraLib.Analytics.Common.Helpers;

namespace SierraLib.Analytics.Common
{
	public class GoogleTracking
	{
		/// <summary>
		/// Tracks the page view  with GA and stream a GIF image
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="urlToTrack">The URL to track.</param>
        public static void TrackPageViewWithImage(HttpWebResponse response)
		{
			//build request
            TrackingRequest request = new RequestFactory().BuildRequest("", "", response.ResponseUri.LocalPath);

			FireTrackingEvent(request);

			ShowTrackingImage(response);

		}
		/// <summary>
		/// Tracks the page view and streams a GIF image.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="pageView">The page view.</param>
		public static void TrackPageViewWithImage(GooglePageView pageView,HttpWebResponse response)
		{
			//build request
			TrackingRequest request = new RequestFactory().BuildRequest(pageView);

			FireTrackingEvent(request);

			ShowTrackingImage(response);
		}

		/// <summary>
		/// Shows the tracking image.
		/// </summary>
		/// <param name="context">The context.</param>
		private static void ShowTrackingImage(HttpWebResponse context)
		{
			//data to show a 1x1 pixel transparent gif
			byte[] GifData = {
					  0x47, 0x49, 0x46, 0x38, 0x39, 0x61,
					  0x01, 0x00, 0x01, 0x00, 0x80, 0xff,
					  0x00, 0xff, 0xff, 0xff, 0x00, 0x00,
					  0x00, 0x2c, 0x00, 0x00, 0x00, 0x00,
					  0x01, 0x00, 0x01, 0x00, 0x00, 0x02,
					  0x02, 0x44, 0x01, 0x00, 0x3b
				  };

			context.ContentType = "image/gif";
			context.Headers.Add("Cache-Control", "private, no-cache, no-cache=Set-Cookie, proxy-revalidate");
			context.Headers.Add("Pragma", "no-cache");
			context.Headers.Add("Expires", "Wed, 17 Sep 1975 21:32:10 GMT");
			context.GetResponseStream().Write(GifData, 0, GifData.Length);
            context.Close();
		}
		/// <summary>
		/// Fires the tracking event with Google Analytics
		/// </summary>
		/// <param name="request">The request.</param>
		public static void FireTrackingEvent(TrackingRequest request)
		{

			//send the request to google
			WebRequest requestForGaGif = WebRequest.Create(request.TrackingGifUri);
			using (WebResponse response = requestForGaGif.GetResponse())
			{
				//ignore response
			}
		}

        public static void FireTrackingEventAsync(TrackingRequest request)
        {
            WebRequest requestForGaGif = WebRequest.Create(request.TrackingGifUri);
            requestForGaGif.BeginGetResponse(new AsyncCallback((o) =>
                { }),null);
        }

        public static void FireTrackingEvent(string domainName, string category, string action, string label, int? value)
        {

            GoogleEvent gaEvent = new SierraLib.Analytics.Common.Data.GoogleEvent(
                domainName,
                category,
                action,
                label,
                value);

            TrackingRequest tracking = new RequestFactory().BuildRequest(gaEvent);

            FireTrackingEvent(tracking);
        }

        public static void FireTrackingEventAsync(string domainName, string category, string action, string label, int? value)
        {

            GoogleEvent gaEvent = new SierraLib.Analytics.Common.Data.GoogleEvent(
                domainName,
                category,
                action,
                label,
                value);

            TrackingRequest tracking = new RequestFactory().BuildRequest(gaEvent);

            FireTrackingEventAsync(tracking);
        }
	}
}
