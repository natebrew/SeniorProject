using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PiggyBank_1.Resources;
using Coding4Fun.Toolkit.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.IO;

namespace PiggyBank_1
{
    public partial class MainPage : PhoneApplicationPage
    {

        private static string fileName = "Assets/Bank.txt";

        // Constructor
        public MainPage()
        {
            double current = readFilein(); 
            current = Math.Round(current, 2, MidpointRounding.AwayFromZero);
                
            
            InitializeComponent();

            balance.Text = Convert.ToString(current);

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        static private double readFilein()
        {
            StreamReader read = new StreamReader(fileName);
            string line = "";

            // I only want to read one line
            line = read.ReadLine();

            read.Close();

            if (line != null)
            {
                try // to convert it to a double and return it
                {
                    return Convert.ToDouble(line);
                }
                catch (FormatException)
                {
                    Console.WriteLine("Unable to convert '{0}' to a Double.", line);
                }
            }

            return 0.0;

        }


        static private bool writeFile(double bal)
        {
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                if (bal >= 0.0)
                {
                    writer.Write(bal);
                    writer.Close();

                    return true;
                }
                else
                    return false;
            }
           
        }

        // the input when the deposit button is clicked
        private void Deposit_Click(object sender, RoutedEventArgs e)
        {
            InputPrompt deposit = new InputPrompt {
                Title = "Amount to Deposit",
                Message = "Format is '00.00'!",
                Background = new SolidColorBrush(Colors.Green)
                };
            deposit.Completed += deposit_Completed;
            deposit.InputScope = new InputScope { Names = { new InputScopeName() { NameValue = InputScopeNameValue.CurrencyAmount } } };
            deposit.Show();
        }

        // the on complete function call
        void deposit_Completed(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            if (e.PopUpResult == PopUpResult.Ok)
            {
                // get the result as a string
                string input = e.Result;
               
                try // to convert it to a double
                {
                    // get the current balance from the display
                    double current = Convert.ToDouble(balance.Text);

                    // get the balance from the file
                    double fileBal = readFilein();

                    // if they are not the same
                    if (current != fileBal)
                    {
                        // take the bigger balance
                        if (fileBal > current)
                            current = fileBal;
                    }

                    // convert the input
                    double num = Convert.ToDouble(input);                   
                    current = current + num;
                    current = TruncateFunction(current, 2);                 
                    balance.Text = Convert.ToString(current); // update the display
                    writeFile(current); //write the new balance to the file

                }
                catch (FormatException)
                {
                    Console.WriteLine("Unable to convert '{0}' to a Double.", input);
                }
                catch (OverflowException)
                {
                    Console.WriteLine("'{0}' is outside the range of a Double.", input);
                }
            }         
        }

        private void Withdrawl_Click(object sender, RoutedEventArgs e)
        {
            InputPrompt with = new InputPrompt
            {
                Title = "Amount to Withdraw",
                Message = "Format is '00.00'!",
                Background = new SolidColorBrush(Colors.Green)
            };
            with.Completed += with_Completed;
            with.InputScope = new InputScope { Names = { new InputScopeName() { NameValue = InputScopeNameValue.CurrencyAmount } } };
            with.Show();
        }

        void with_Completed(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            if (e.PopUpResult == PopUpResult.Ok)
            {
                string input = e.Result;

                try
                {
                    // get the current balance from the display
                    double current = Convert.ToDouble(balance.Text);

                    // get the balance from the file
                    double fileBal = readFilein();

                    // if they are not the same
                    if (current != fileBal)
                    {
                        // take the bigger balance
                        if (fileBal > current)
                            current = fileBal;
                    }

                    // convert the input
                    double num = Convert.ToDouble(input);
                    // truncate it before subtracting it
                    num = TruncateFunction(num, 2);
                    current = current - num;
                    current = TruncateFunction(current, 2);
                    // never go below 0.0
                    if (current < 0.0)
                        current = 0.0;

                    balance.Text = Convert.ToString(current); // update the display
                    writeFile(current); //write the new balance to the file
                   
                }
                catch (FormatException)
                {
                    Console.WriteLine("Unable to convert '{0}' to a Double.", input);
                }
                catch (OverflowException)
                {
                    Console.WriteLine("'{0}' is outside the range of a Double.", input);
                }
            }         
        }

        public double TruncateFunction(double number, int digits)
        {
            double stepper = (Math.Pow(10.0, (double)digits));
            int temp = (int)(stepper * number);
            return temp / stepper;
        }

        private void Camera_Click(object sender, RoutedEventArgs e)
        {
            //var messagePrompt = new MessagePrompt
            //{
            //    Title = "Simple Message",
            //    Message = "This is just to show you that clicking this button can do something :) Have a nice Day!"
            //};
            
            //messagePrompt.Show();

            NavigationService.Navigate(new Uri("/CameraPage.xaml", UriKind.RelativeOrAbsolute));

        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}