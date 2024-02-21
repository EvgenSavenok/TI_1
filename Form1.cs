using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.LinkLabel;
using System.Security.Authentication;
using System.Diagnostics.SymbolStore;

namespace TI_1
{
    public partial class MainForm : Form
    {
        char[,] russianTable = null;
        static Dictionary<int, int> keyMapping = new Dictionary<int, int>()
        {
            { 3, 9 },
            { 5, 21 },
            { 7, 15 },
            { 9, 3 },
            { 11, 19},
            { 15, 7},
            { 17, 23},
            { 19, 11},
            { 21, 5},
            { 23, 17},
            { 25, 25},
        };
        public MainForm()
        {
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
            InitializeButton();
            InitializeComboBox();
        }
        private void InitializeComboBox()
        {
            comboBox.SelectedItem = "Метод децимаций";
        }
        private void InitializeButton()
        {        
            encryptBtn.Enabled = false;
        }
        private void inputKeyTextBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ((System.Windows.Forms.TextBox)sender).ContextMenuStrip = null;
            }    
        }

        private void inputKeyTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void inputKeyTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(inputKeyTextBox.Text) && !string.IsNullOrWhiteSpace(textBox.Text))
            {             
                encryptBtn.Enabled = true;
                decipherBtn.Enabled = true;
            }
            else
            {
                decipherBtn.Enabled = false;
                encryptBtn.Enabled = false;
            }
            SaveFile.Enabled = false;
        }

        private void AboutDeveloper_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Евгений Савенок, группа 251004");
        }

        public string ReadFromFile(string path)
        {
            string line = null;
            string text = null;
            try
            {
                StreamReader sr = new StreamReader(path);
                line = sr.ReadLine();
                while (line != null)
                {
                    text += line;
                    line = sr.ReadLine();
                }
                sr.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Error of reading file!");
            }
            return text;
        }
        public string HandleOpenedFile()
        {
            string openedFileName = null;
            string text = null;
            DialogResult result = openFileDialog.ShowDialog();
            if ((result == DialogResult.OK) && (result != DialogResult.Cancel))
            {
                openedFileName = openFileDialog.FileName;
                text = ReadFromFile(openedFileName);
                return text;
            }
            return null;
        }
        static bool FindNOD(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a == 1;
        }
        public string StartDecimationMethod(string plaintext, int key)
        {
            const int NUM_OF_LATINIC_SYMBOLS = 26;
            StringBuilder encryptedString = new StringBuilder();
            foreach (char c in plaintext)
            {
                if (c >= 'a' && c <= 'z')
                {
                    int symbol = (int)c - 'a';
                    int encryptedCharValue = (symbol * key) % NUM_OF_LATINIC_SYMBOLS;
                    char encryptedChar = (char)(encryptedCharValue + 'a');
                    encryptedString.Append(encryptedChar);
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    int symbol = (int)c - 'A';
                    int encryptedCharValue = (symbol * key) % NUM_OF_LATINIC_SYMBOLS;
                    char encryptedChar = (char)(encryptedCharValue + 'A');
                    encryptedString.Append(encryptedChar);
                }
                else
                { 
                    encryptedString.Append(c);
                }
            }
            return encryptedString.ToString();
        }
        private void OpenFile_Click(object sender, EventArgs e)
        {
            string text = null;
            text = HandleOpenedFile();
            if (text != null)
            {
                textBox.Text = text;   
            }
        }
        private void textBox_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(inputKeyTextBox.Text) && !string.IsNullOrWhiteSpace(textBox.Text))
            {
                encryptBtn.Enabled = true;
                decipherBtn.Enabled = true;
            }
            else
            {
                encryptBtn.Enabled = false;
                decipherBtn.Enabled = false;
            }
            SaveFile.Enabled = false;
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        public char[] ConvertToChars(int[] numbers)
        {
            char[] letters = new char[numbers.Length];
            for (int i = 0; i < numbers.Length; i++)
            {
                if (numbers[i] > 0 && numbers[i] < 27)
                {
                    int asciiValue = numbers[i] + 97;
                    letters[i] = (char)asciiValue;
                }
            }
            return letters; 
        }
        public string HandleDecimationMethod()
        {
            const int NUM_OF_LATINIC_CHARS = 26;
            int key = Convert.ToInt32(inputKeyTextBox.Text);
            string plaintext = textBox.Text;
            if (key > 25 || key < 3)
            {
                MessageBox.Show("Недопустимый диапазон ввода для ключа (от 3 до 25)!");
                return null;
            }
            bool isOkNOD = FindNOD(NUM_OF_LATINIC_CHARS, key);
            if (isOkNOD)
            {
                encryptBtn.Enabled = false;
                decipherBtn.Enabled = true;
                return StartDecimationMethod(plaintext, key);
            }
            else
            {
                MessageBox.Show("Числа не должны быть взаимно простыми!");
                return null;
            }
        }
        public char[,] GetRussianTable()
        {
            const int RUSSIAN_ALPHABET_LENGTH = 33;
            char[,] russianTable = new char[RUSSIAN_ALPHABET_LENGTH, RUSSIAN_ALPHABET_LENGTH];
            for (int i = 0; i < RUSSIAN_ALPHABET_LENGTH; i++)
            {
                for (int j = 0; j < RUSSIAN_ALPHABET_LENGTH; j++)
                {
                    int unicode = 'а' + (i + j) % RUSSIAN_ALPHABET_LENGTH;
                    russianTable[i, j] = (char)unicode;
                }
            }
            return russianTable;
        }
        public char[,] RelateKeyAndPlaintext(string plaintext, string key)
        {
            int textLength = plaintext.Length;
            int keyLength = key.Length;
            char[,] vigenereTable = new char[2, textLength];
            for (int i = 0; i < textLength; i++)
            {
                vigenereTable[0, i] = plaintext[i];
            }
            for (int i = 0; i < textLength; i++)
            {
                char keyChar = key[i % keyLength];
                vigenereTable[1, i] = keyChar;
            }
            return vigenereTable;
        }
        public string getCipherString(char[,] russianTable, char[,] vigenereDictionary, string plaintext)
        {
            StringBuilder cipherString = new StringBuilder();
            int plaintextCharIndex = 0;
            for (int i = 0; i < plaintext.Length; i++)
            {
                if ((plaintext[i] >= 'а' && plaintext[i] <= 'я') || plaintext[i] == 'ё')
                {
                    if (plaintext[i] == 'ё')
                        plaintextCharIndex = vigenereDictionary[0, i] - 'а' - 1;
                    else
                        plaintextCharIndex = vigenereDictionary[0, i] - 'а';
                    int keyCharIndex = vigenereDictionary[1, i] - 'а';
                    cipherString.Append(russianTable[plaintextCharIndex, keyCharIndex]);
                }
                else if ((plaintext[i] >= 'А' && plaintext[i] <= 'Я') || (plaintext[i] == 'Ё'))
                {
                    if (plaintext[i] == 'Ё')
                        plaintextCharIndex = char.ToLower(vigenereDictionary[0, i]) - 'а' - 1;
                    else
                        plaintextCharIndex = char.ToLower(vigenereDictionary[0, i]) - 'а';
                    int keyCharIndex = vigenereDictionary[1, i] - 'а';
                    cipherString.Append(char.ToUpper(russianTable[plaintextCharIndex, keyCharIndex]));                 
                }
                else
                {
                    cipherString.Append(plaintext[i]);
                }
            }
            return cipherString.ToString();
        }
        public string decipherVigenere(string cipherString, string key, char[,] vigenereDictionary, char[,] russianTable)
        {
            StringBuilder decipherString = new StringBuilder();
            const int NUM_OF_RUSSIAN_SYMBOLS = 33;
            for (int i = 0; i < cipherString.Length; i++)
            {
                if (cipherString[i] >= 'а' && cipherString[i] <= 'я')
                {
                    int indexOfKeyChar = (int)((vigenereDictionary[1, i]) - 'а');
                    for (int j = 0; j < NUM_OF_RUSSIAN_SYMBOLS; j++)
                    {
                        if (russianTable[indexOfKeyChar, j] == cipherString[i])
                        {
                            if (j == ('ё' - 'а' - 1))
                                decipherString.Append('ё');
                            else
                                decipherString.Append((char)(j + 'а'));
                        }
                    }
                }
                else if (cipherString[i] >= 'А' && cipherString[i] <= 'Я')
                {
                    int indexOfKeyChar = (int)(char.ToLower(vigenereDictionary[1, i]) - 'а');
                    for (int j = 0; j < NUM_OF_RUSSIAN_SYMBOLS; j++)
                    {
                        if (char.ToLower(russianTable[indexOfKeyChar, j]) == char.ToLower(cipherString[i]))
                        {
                            if (j == ('ё' - 'а' - 1))
                                decipherString.Append('Ё');
                            else
                                decipherString.Append(char.ToUpper((char)(j + 'а')));
                        }
                    }
                }
                else
                {
                    decipherString.Append(cipherString[i]);
                }
            }
            return decipherString.ToString();

        }
        public string HandleVijinerCipher()
        {
            const int RUSSIAN_ALPHABET_LENGTH = 33;
            this.russianTable = new char[RUSSIAN_ALPHABET_LENGTH, RUSSIAN_ALPHABET_LENGTH];
            this.russianTable = GetRussianTable();
            string key = inputKeyTextBox.Text.ToLower();
            string plaintext = textBox.Text;
            char[,] vigenereDictionary = RelateKeyAndPlaintext(plaintext, key);
            return getCipherString(this.russianTable, vigenereDictionary, plaintext);
        }
        public void WriteAnswerToTB(string cipheredArrOfNumbers)
        {
            textBox.Text = null;
            for (int i = 0; i < cipheredArrOfNumbers.Length; i++)
                textBox.Text += cipheredArrOfNumbers[i];
            if (!string.IsNullOrWhiteSpace(textBox.Text))
                SaveFile.Enabled = true;
        }
        private void inputKeyBtn_Click(object sender, EventArgs e)
        {
            string cipheredArrOfNumbers = null;
            encryptBtn.Enabled = false;
            if (comboBox.SelectedItem.ToString() == "Метод децимаций")
                cipheredArrOfNumbers = HandleDecimationMethod();
            else if (comboBox.SelectedItem.ToString() == "Шифр Вижинера")
                cipheredArrOfNumbers = HandleVijinerCipher();
            if (cipheredArrOfNumbers != null)
            {
                decipherBtn.Enabled = true;
                WriteAnswerToTB(cipheredArrOfNumbers);
            }
        }
        private void inputKeyTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (comboBox.SelectedIndex == 0)
            {
                if (!(e.KeyChar >= '0' && e.KeyChar <= '9') && (e.KeyChar != '\b'))
                {
                    e.Handled = true;
                }
            }
            if (comboBox.SelectedIndex == 1)
            {
                if (!(e.KeyChar >= 'а' && e.KeyChar <= 'я') && (e.KeyChar != '\b'))
                {
                    e.Handled = true;
                }
            }
        }
        public void WriteToFile(string path, string cipheredText)
        {
            using (StreamWriter outputFile = new StreamWriter(path))
            {
                outputFile.WriteLine(cipheredText);
            }
        }
        private void HandleSavedFile()
        {
            string savedFileName = null;
            string cipheredText = textBox.Text;
            DialogResult result = saveFileDialog.ShowDialog();
            if ((result == DialogResult.OK) && (result != DialogResult.Cancel))
            {
                savedFileName = saveFileDialog.FileName;
                WriteToFile(savedFileName, cipheredText);
            }
        }
        private void SaveFile_Click(object sender, EventArgs e)
        {
            HandleSavedFile();
        }
        private void inputKeyTextBox_KeyDown_1(object sender, KeyEventArgs e)
        {
            decipherBtn.Enabled = false;
        }
        public string StartDecodingDecimation(int key, string cipherText)
        {
            const int NUM_OF_LATINIC_SYMBOLS = 26;
            StringBuilder encryptedString = new StringBuilder();
            foreach (char c in cipherText)
            {
                if (c >= 'a' && c <= 'z')
                {
                    int decipherKey = keyMapping[key];
                    int symbol = (int)c - 'a';
                    int encryptedCharValue = (symbol * decipherKey) % NUM_OF_LATINIC_SYMBOLS;
                    char encryptedChar = (char)(encryptedCharValue + 'a');
                    encryptedString.Append(encryptedChar);
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    int decipherKey = keyMapping[key];
                    int symbol = (int)c - 'A';
                    int encryptedCharValue = (symbol * decipherKey) % NUM_OF_LATINIC_SYMBOLS;
                    char encryptedChar = (char)(encryptedCharValue + 'A');
                    encryptedString.Append(encryptedChar);
                }
                else
                {
                    encryptedString.Append(c);
                }
            }
            return encryptedString.ToString();
        }
        public void StartDecoding()
        {
            string plaintext = null;
            string cipherText = textBox.Text;
            if (comboBox.SelectedIndex == 0)
            {
                int key;
                key = Convert.ToInt32(inputKeyTextBox.Text);
                plaintext = StartDecodingDecimation(key, cipherText);

            }
            else if (comboBox.SelectedIndex == 1)
            {
                this.russianTable = GetRussianTable();
                string key = inputKeyTextBox.Text.ToLower();
                char[,] vigenereDictionary = RelateKeyAndPlaintext(cipherText, key);
                plaintext = decipherVigenere(cipherText, key, vigenereDictionary, this.russianTable);
            }
            textBox.Text = plaintext;
            SaveFile.Enabled = true;
        }
        private void decipherBtn_Click(object sender, EventArgs e)
        {
            StartDecoding();
        }
        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void comboBox_Click(object sender, EventArgs e)
        {
            comboBox.Text = null;
            inputKeyTextBox.Text = null;
            textBox.Text = null;
            decipherBtn.Enabled = false;
        }
    }
}
