using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Devices;
using Microsoft.Xna.Framework.Media;
using Microsoft.Phone.Data.Linq;
using Microsoft.Phone.Data.Linq.Mapping;
using System.IO.IsolatedStorage;
using System.IO;
using System.Windows.Media;

namespace PiggyBank_1
{
    public partial class CameraPage : PhoneApplicationPage
    {

        private int savedCounter = 0;
        PhotoCamera cam;
        MediaLibrary library = new MediaLibrary();

        public CameraPage()
        {
            InitializeComponent();
        }

        //Code for initialization, capture completed, image availability events; also setting the source for the viewfinder.
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Check to see if the camera is available on the phone.
            if (PhotoCamera.IsCameraTypeSupported(CameraType.Primary) == true)
            {
                // Initialize the camera, when available.
                cam = new Microsoft.Devices.PhotoCamera(CameraType.Primary);

                // Event is fired when the PhotoCamera object has been initialized.
                cam.Initialized += new EventHandler<Microsoft.Devices.CameraOperationCompletedEventArgs>(cam_Initialized);

                // Event is fired when the capture sequence is complete.
                cam.CaptureCompleted += new EventHandler<CameraOperationCompletedEventArgs>(cam_CaptureCompleted);

                // Event is fired when the capture sequence is complete and an image is available.
                cam.CaptureImageAvailable += new EventHandler<Microsoft.Devices.ContentReadyEventArgs>(cam_CaptureImageAvailable);

                // Event is fired when the capture sequence is complete and a thumbnail image is available.
                cam.CaptureThumbnailAvailable += new EventHandler<ContentReadyEventArgs>(cam_CaptureThumbnailAvailable);

                // The event is fired when autofocus is complete.
                cam.AutoFocusCompleted += new EventHandler<CameraOperationCompletedEventArgs>(cam_AutoFocusCompleted);

                //// The event is fired when the viewfinder is tapped (for focus).
                //viewfinderCanvas.Tap += new EventHandler<GestureEventArgs>(focus_Tapped);

                //Set the VideoBrush source to the camera.
                viewfinderBrush.SetSource(cam);
            }
            else
            {
                // The camera is not supported on the phone.
                this.Dispatcher.BeginInvoke(delegate()
                {
                    // Write message.
                    txtDebug.Text = "A Camera is not available on this phone.";
                });

                // Disable UI.
                ShutterButton.IsEnabled = false;
                focusButton.IsEnabled = false;
                backButton.IsEnabled = false;
            }
        }

        void cam_CaptureCompleted(object sender, CameraOperationCompletedEventArgs e)
        {
            // Increments the savedCounter variable used for generating JPEG file names.
            savedCounter++;
        }

        // Informs when full resolution photo has been taken, saves to local media library and the local folder.
        void cam_CaptureImageAvailable(object sender, Microsoft.Devices.ContentReadyEventArgs e)
        {
            string fileName = savedCounter + ".jpg";

            try
            {   // Write message to the UI thread.
                Deployment.Current.Dispatcher.BeginInvoke(delegate()
                {
                    txtDebug.Text = "Captured image available, saving photo.";
                });

                // Save photo to the media library camera roll.
                library.SavePictureToCameraRoll(fileName, e.ImageStream);

                // Write message to the UI thread.
                Deployment.Current.Dispatcher.BeginInvoke(delegate()
                {
                    txtDebug.Text = "Photo has been saved to camera roll.";
                });

                // Set the position of the stream back to start
                e.ImageStream.Seek(0, SeekOrigin.Begin);

                // Save photo as JPEG to the local folder.
                using (IsolatedStorageFile isStore = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream targetStream = isStore.OpenFile(fileName, FileMode.Create, FileAccess.Write))
                    {
                        // Initialize the buffer for 4KB disk pages.
                        byte[] readBuffer = new byte[4096];
                        int bytesRead = -1;

                        // Copy the image to the local folder. 
                        while ((bytesRead = e.ImageStream.Read(readBuffer, 0, readBuffer.Length)) > 0)
                        {
                            targetStream.Write(readBuffer, 0, bytesRead);
                        }
                    }
                }

                // Write message to the UI thread.
                Deployment.Current.Dispatcher.BeginInvoke(delegate()
                {
                    txtDebug.Text = "Photo has been saved to the local folder.";
                });
            }
            finally
            {
                // Close image stream
                e.ImageStream.Close();
            }

        }

        // Informs when thumbnail photo has been taken, saves to the local folder
        // User will select this image in the Photos Hub to bring up the full-resolution. 
        public void cam_CaptureThumbnailAvailable(object sender, ContentReadyEventArgs e)
        {
            string fileName = savedCounter + "_th.jpg";

            try
            {
                // Write message to UI thread.
                Deployment.Current.Dispatcher.BeginInvoke(delegate()
                {
                    txtDebug.Text = "Captured image available, saving thumbnail.";
                });

                // Save thumbnail as JPEG to the local folder.
                using (IsolatedStorageFile isStore = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream targetStream = isStore.OpenFile(fileName, FileMode.Create, FileAccess.Write))
                    {
                        // Initialize the buffer for 4KB disk pages.
                        byte[] readBuffer = new byte[4096];
                        int bytesRead = -1;

                        // Copy the thumbnail to the local folder. 
                        while ((bytesRead = e.ImageStream.Read(readBuffer, 0, readBuffer.Length)) > 0)
                        {
                            targetStream.Write(readBuffer, 0, bytesRead);
                        }
                    }
                }

                // Write message to UI thread.
                Deployment.Current.Dispatcher.BeginInvoke(delegate()
                {
                    txtDebug.Text = "Thumbnail has been saved to the local folder.";
                });
            }
            finally
            {
                // Close image stream
                e.ImageStream.Close();
            }
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (cam != null)
            {
                // Dispose camera to minimize power consumption and to expedite shutdown.
                cam.Dispose();
                cam.AutoFocusCompleted -= cam_AutoFocusCompleted;

                // Release memory, ensure garbage collection.
                cam.Initialized -= cam_Initialized;
                cam.CaptureCompleted -= cam_CaptureCompleted;
                cam.CaptureImageAvailable -= cam_CaptureImageAvailable;
                cam.CaptureThumbnailAvailable -= cam_CaptureThumbnailAvailable;
            }
        }

        void cam_AutoFocusCompleted(object sender, CameraOperationCompletedEventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(delegate()
            {
                // Write message to UI.
                txtDebug.Text = "Autofocus has completed.";
                // Hide the focus brackets.
                focusBrackets.Visibility = Visibility.Collapsed;
            });
        }

        // Update the UI if initialization succeeds.
        void cam_Initialized(object sender, Microsoft.Devices.CameraOperationCompletedEventArgs e)
        {
            if (e.Succeeded)
            {
                this.Dispatcher.BeginInvoke(delegate()
                {
                    // Write message.
                    txtDebug.Text = "Camera initialized.";
                });
            }
        }

        // Ensure that the viewfinder is upright in LandscapeRight.
        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            if (cam != null)
            {
                // LandscapeRight rotation when camera is on back of phone.
                int landscapeRightRotation = 180;

                // Change LandscapeRight rotation for front-facing camera.
                if (cam.CameraType == CameraType.FrontFacing) landscapeRightRotation = -180;

                // Rotate video brush from camera.
                if (e.Orientation == PageOrientation.LandscapeRight)
                {
                    // Rotate for LandscapeRight orientation.
                    viewfinderBrush.RelativeTransform =
                        new CompositeTransform() { CenterX = 0.5, CenterY = 0.5, Rotation = landscapeRightRotation };
                }
                else
                {
                    // Rotate for standard landscape orientation.
                    viewfinderBrush.RelativeTransform =
                        new CompositeTransform() { CenterX = 0.5, CenterY = 0.5, Rotation = 0 };
                }
            }

            base.OnOrientationChanged(e);
        }

        //
        //  The BUTTONS and the click events
        //

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            // navigate back to the home page
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void ShutterButton_Click(object sender, RoutedEventArgs e)
        {
            if (cam != null)
            {
                try
                {
                    // Start image capture.
                    cam.CaptureImage();

                    //
                    //process image here
                    //


                    // navigate back to the home page
                    //NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute));


                }
                catch (Exception ex)
                {
                    this.Dispatcher.BeginInvoke(delegate()
                    {
                        // Cannot capture an image until the previous capture has completed.
                        txtDebug.Text = ex.Message;
                    });
                }
            }
        }

        private void focusButton_Click(object sender, RoutedEventArgs e)
        {
            if (cam.IsFocusSupported == true)
            {
                //Focus when a capture is not in progress.
                try
                {
                    cam.Focus();
                }
                catch (Exception focusError)
                {
                    // Cannot focus when a capture is in progress.
                    this.Dispatcher.BeginInvoke(delegate()
                    {
                        txtDebug.Text = focusError.Message;
                    });
                }
            }
            else
            {
                // Write message to UI.
                this.Dispatcher.BeginInvoke(delegate()
                {
                    txtDebug.Text = "Camera does not support programmable autofocus.";
                });
            }
        }
    }
}