//////////////////////////////////////////////////////////////
// This Project uses code from https://dotnettutorials.net/ //
//            and https://learn.microsoft.com               //
//                                                          //
//                      Made by Dynwares                    //
//         This project uses GNU GPL Software License       //
//                                                          //
//////////////////////////////////////////////////////////////
// You can find the License file in the ./license/ directory

using System;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ToBinary;
using Microsoft.Maui;

namespace PassGen;

public partial class MainPage : ContentPage
{
    string completePassword;
    string password;
    public MainPage()
    {
        InitializeComponent();
    }


    // STEP 0 : Reciving event

    //
    public void GeneratePassword(object sender, EventArgs e)
    {


        DateTime localDate = DateTime.Now;


        // Type Conversion
        password = localDate.ToString();
        password = password.ToBinary();
        password = BinaryToDecimal(password);

        // Manipulation
        password = NumberManipulation(password);

        // Displaying
        PasswordLabel.FontAttributes = FontAttributes.None;
        PasswordLabel.Text = password;
        completePassword = password;
        // Makes the "Copy" button useable
        CopyButton.Opacity = 1;
        CopyButton.IsEnabled = true;

        // Reset the state of the "Copy" button (AESTHETIC)
        CopyButton.FontAttributes = FontAttributes.None;
        CopyButton.Text = "Copy";

        // Initialises the lenght limit
        LenghtLimit.Maximum = password.Length;
        LenghtLimit.Value = password.Length;
        LenghtLimit.IsVisible = true;
        SliderValue.IsVisible = true;
    }



    // STEP 1 : Date -> Binary -> Decimal

    //
    public string BinaryToDecimal(string binaryNumberArgument)    // Code from https://dotnettutorials.net/
    {
        BigInteger binaryNumber = BigInteger.Parse(binaryNumberArgument);
        BigInteger decimalValue = 0;
        // initializing base1 value to 1, i.e 2^0 
        BigInteger base1 = 1;

        while (binaryNumber > 0)
        {
            BigInteger reminder = (binaryNumber % 10);
            binaryNumber = binaryNumber / 10;
            decimalValue += reminder * base1;
            base1 = base1 * 2;
        }

        return decimalValue.ToString();
    }

    public string NumberManipulation(string passwordToManipulate)
    {
        passwordToManipulate = Hashing(passwordToManipulate); // STEP 2 : Hashing
        passwordToManipulate = RandomChars(passwordToManipulate); // STEP 3 : Random Characters
        passwordToManipulate = RandomCase(passwordToManipulate); // STEP 4 : Random lower and UPPER case

        return passwordToManipulate;
    }




    // STEP 2 : Hashing

    // WARNING ! Quasi all the following code was taken from https://bit.ly/3KQJyOW (Microsoft Learn)
    public string Hashing(string number) //Create a hash of the password (MD5) 
    {
        //
        byte[] tmpSource;
        byte[] tmpHash;

        //Encoding number
        tmpSource = Encoding.ASCII.GetBytes(number);

        //Compute hash based on source data.
        tmpHash = MD5.HashData(tmpSource);

        return ByteArrayToString(tmpHash);
    }
    static string ByteArrayToString(byte[] arrInput) // Transforms the byte array of the hash to a String
    {
        int i;
        StringBuilder sOutput = new StringBuilder(arrInput.Length);
        for (i = 0; i < arrInput.Length; i++)
        {
            sOutput.Append(arrInput[i].ToString("X2"));
        }
        return sOutput.ToString();
    }




    // STEP 3 : Random Characters

    //
    public string RandomChars(string simplePassword) // Complicate the Password by adding special characters
    {
        // Complicated Char Tables
        string[] complicatedCharsTable = { "&", "é", "#", "{", "[", "|", "`", "(", "^", "ç", "@", "}", "=", "+"
                                            , "€", "?", ";", ".", "!", "§", "%", "[", "]", "°", "*", "$", "£", "¤"};


        string complicatedPassword = simplePassword;

        //Initializing number clients
        Random NumberOfSpecialCharRandomClient = new Random();
        Random randomComplicatedChars = new Random();


        int numberOfSpecialChars = NumberOfSpecialCharRandomClient.Next(simplePassword.Length);
        for (int i = 0; i < numberOfSpecialChars; i++)
        {
            // Chooses a random Complicated Char
            int complicatedCharsIndex = randomComplicatedChars.Next(complicatedCharsTable.Length);

            // Inserts a random character from the Char Table to the place i + the numbers of leters added
            complicatedPassword = complicatedPassword.Insert(i + complicatedPassword.Length - simplePassword.Length, complicatedCharsTable[complicatedCharsIndex]);
        }

        return complicatedPassword;
    }


    // STEP 4 : Random lower and UPPER case

    //
    public string RandomCase(string simplePassword)
    {
        // Initialize the password that will be modified
        char[] complicatedPassword = simplePassword.ToCharArray();

        // Initialize the random Client
        Random randomLoweringCharsClient = new Random();


        // Loop that goes in all the chars of the password
        for (int i = 1; i < simplePassword.Length; i++)
        {

            // Check if the char can be Upper or Lower Case
            string testedChar = simplePassword[i].ToString();
            if (testedChar.ToUpper() != testedChar.ToLower())
            {
                // 1 out of 2 chances to Lower the case
                if (randomLoweringCharsClient.Next(2) == 1)
                {

                    // Verifies if the char is Upper or Lower
                    // It Lowers it when it's upper and upper it when it's lower
                    if (testedChar == testedChar.ToLower())
                    {
                        testedChar = testedChar.ToUpper();
                    }
                    else
                    {
                        testedChar = testedChar.ToLower();
                    }

                    // Insert the char to the string
                    complicatedPassword[i] = char.Parse(testedChar);
                }
                else
                {
                    complicatedPassword[i] = simplePassword[i];
                }

            }

        }

        // Transforming the output from a char[] to a string
        string output = new string(complicatedPassword);
        return output;
    }


    // STEP 5 (Optional) : Copy to the Clipboard

    //

    public void CopyGeneratedPassword(object sender, EventArgs e)
    {
        Clipboard.SetTextAsync(PasswordLabel.Text);

        // Ahestetic (know when the button has been pressed)
        CopyButton.Text = "Copied !";
        CopyButton.FontAttributes = FontAttributes.Italic;

    }

    // Lenght Limit
    //

    public void LimitLenght(int limit)
    {
        // Takes the Complete password as a base to not have an OutOfBounds Exception
        string passwordToModify = completePassword;
        passwordToModify = passwordToModify.Remove(limit);

        // Sets the modified password to PasswordLabel & it's lenght to SliderValue
        PasswordLabel.Text = password = passwordToModify;
        SliderValue.Text = "Lenght = " + passwordToModify.Length;

    }

    private void LenghtLimit_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        LimitLenght(Convert.ToInt32(LenghtLimit.Value));
    }
}