using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace wordsearch_gensolver
{
    class wordsearch_gensolver
    {
        static void Main(string[] args)
        {
            const int horizontal_line_times = 110;
            
            Console.WriteLine("\n");
            char_loop_print(horizontal_line_times,'~');

            if (args.Length == 0) { // Pre-program argument check
                Console.WriteLine("No arguments entered. Type './wordsearch_gensolver help' to get help on available commands.");
                Console.Write("Press any key to continue . . .");
                Console.ReadKey();
                return;
            }

            string program_choice = args[0];

            // Lettermap and wordlist arrays
            char?[,] lettermap = null;
            char?[,] solution_lettermap = null;
            char?[,] generate_lettermap = null;
            string[] wordlist = null;

            // single-word processing variables
            char[] word_array = null;
            int word_length = -1;
            string tempword;

            // jsut the size
            int size = 0;

            // Solving variables
            int word_index = 0;
            int matching_start_index = 0;
            int matching_row_index = -1;
            int matching_col_index = -1;

            // Unfound words
            List<string> unfound_words = new List<string>();

            switch (program_choice)
            { // arg 0 is the program option
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                case "generate": // syntax: program generate *size* *filename*
                    // # of arguments check
                    if (args.Length != 3) { // Pre-program argument check
                        Console.WriteLine("Not enough arguments entered. Type './wordsearch_gensolver help' to get help on the generate command.");
                        Console.Write("Press any key to continue . . .");
                        Console.ReadKey();
                        return;
                    }

                    string input_filename = args[2] + ".txt";
                    size = Convert.ToInt32(args[1]);
                    generate_lettermap = new char?[size, size];
                    solution_lettermap = new char?[size,size];

                    // Wordlist file handling
                    try {
                        using (StreamReader sr = new StreamReader(input_filename)) {
                            string line = sr.ReadLine();
                            wordlist = line.Split(' '); // parse last line as the words to find and store into a separate 1D array
                            Array.Sort<string>(wordlist); // word list put sorted in the wordlist array
                        }
                    }
                    catch (IOException e) {
                        Console.WriteLine("The file " + input_filename + " could not be opened. Reason:");
                        Console.WriteLine(e.Message);
                        Console.WriteLine("Exiting . . .");
                        return;
                    }
                    
                    // Lettermap generation
                    var rand1 = new Random((int)DateTime.Now.Ticks);
                    var rand2 = new Random((int)DateTime.Now.Minute);
                    var rand3 = new Random();
                    var rand4 = new Random();

                    int rand_row = -1;
                    int rand_col = -1;
                    int rand_direction = 0;
                    int rand_char_val = -1;

                    bool verify_passfail = false;
                    
                    char[] blanks = null;
                    
                    // populate the lettermap with just the words from the wordlist
                    foreach (var word in wordlist) {
                        tempword = word.ToUpper();
                        word_array = tempword.ToCharArray();
                        word_length = word.Length;
                        
                        //fill the blanks with just ' ' chars.
                        blanks = null;
                        blanks = new char[word_length];
                        for (int i = 0; i < word_length; i++) {
                            blanks[i] = ' ';
                        }
                        
                        // generate random row and col index for the first letter
                        while (!verify_passfail) { // loop until the verify passes.
                            rand_row = rand1.Next(0,size);
                            rand_col = rand2.Next(0,size);
                            rand_direction = rand3.Next(1,9); //1-8 inclusive

                            verify_passfail = verify_lettermap_spaceforword(generate_lettermap, word_array, rand_row, rand_col, rand_direction);
                        }
                        
                        populate_lettermap_with_word(generate_lettermap, word_array, rand_row, rand_col, rand_direction);
                        populate_lettermap_with_word(solution_lettermap, blanks, rand_row, rand_col, rand_direction);
                        verify_passfail = false;
                    }
                    // populate the null entreis in the array with random other letters;
                    for (int i = 0; i < size; i++) {
                        for (int j = 0; j < size; j++) {
                            rand_char_val = rand4.Next(65, 91); 
                            if (!generate_lettermap[i,j].HasValue) {
                                generate_lettermap[i,j] = (char)rand_char_val;
                                solution_lettermap[i,j] = (char)rand_char_val;
                            }
                        }
                    }

                    //Printing the generated lettermap and solution lettermap to separate files.
                    string wordsearch_name = args[2] + "_wordsearch.txt";
                    string wordsearch_solution_name = args[2] + "_wordsearch_solution.txt";

                    // generated lettermap
                    if (File.Exists(wordsearch_name)) { //check for existing ones
                        File.Delete(wordsearch_name);
                    }
                    try {
                        using (StreamWriter outputfile = new StreamWriter("./"+ wordsearch_name)) {
                            for (int i = 0; i < size; i++) {
                                for (int j = 0; j < size; j++) {
                                    outputfile.Write(generate_lettermap[i,j] + " ");
                                }
                                outputfile.Write("\n");
                            }
                            foreach(var word in wordlist) {
                                outputfile.Write(word + " ");
                            }
                        }
                    }
                    catch (IOException e) {
                        Console.WriteLine(">> The solution file " + wordsearch_name + " could not be produced. Reason:");
                        Console.WriteLine(e.Message);
                        Console.WriteLine("Exiting . . .");
                        return;
                    }
                    
                    //solution lettermap
                    if (File.Exists(wordsearch_solution_name)) { //check for existing ones
                        File.Delete(wordsearch_solution_name);
                    }
                    try {
                        using (StreamWriter outputfile = new StreamWriter("./"+ wordsearch_solution_name)) {
                            for (int i = 0; i < size; i++) {
                                for (int j = 0; j < size; j++) {
                                    outputfile.Write(solution_lettermap[i,j] + " ");
                                }
                                outputfile.Write("\n");
                            }
                        }
                    }
                    catch (IOException e) {
                        Console.WriteLine(">> The solution file " + wordsearch_solution_name + " could not be produced. Reason:");
                        Console.WriteLine(e.Message);
                        Console.WriteLine("Exiting . . .");
                        return;
                    }

                    Console.WriteLine(">> Wordsearch lettermap generated successfully. See {0}", wordsearch_name);
                    Console.WriteLine(">> Wordsearch solution generated successfully. See {0}", wordsearch_solution_name);
                    break;
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                case "solve": // syntax program solve *filename*
                    string filename = args[1] + ".txt";
                    string path = "./" + filename;

                    string solution_name = args[1] + "_solution.txt";
                    string error_log_name = args[1] + "_log.txt";

                    // Extracting the lettermap and wordlist from the .txt file
                    try
                    {
                        using (StreamReader sr = new StreamReader(filename)) // open file
                        {
                            string line = sr.ReadLine(); // parse first line to see number of letters (N) = size of crossword
                            line = line.Replace(" ",String.Empty); // remove spaces if they are present
                            size = line.Length;
                            lettermap = new char?[size, size]; // create a 2D array with size N
                            solution_lettermap = new char?[size,size];

                            char[] row_array = null;
                            // read all N lines and store each char into the 2D array
                            for (int i = 0; i < size ; i++) {
                                line = line.Replace(" ",String.Empty); // remove spaces if they are present
                                row_array = line.ToCharArray();
                                for (int j = 0; j < size; j++)
                                {
                                    lettermap[i, j] = row_array[j];
                                    solution_lettermap[i, j] = row_array[j];
                                }
                                line = sr.ReadLine();
                            } 
                            //print_2d_char_array(lettermap, size); // Array print check

                            // Last line read in should contain the words to be found.
                            wordlist = line.Split(' ');// parse last line as the words to find and store into a separate 1D array
                            Array.Sort<string>(wordlist);
                            //print_1d_string_array(wordlist, wordlist.Length); // word extract check
                        }
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine("The file " + filename + " could not be opened. Reason:");
                        Console.WriteLine(e.Message);
                        Console.WriteLine("Exiting . . .");
                        return;
                    }
                    
                    // Solving the wordsearch
                    foreach (var word in wordlist) // traversing the crossword for each word
                    {
                        tempword = word.ToUpper();
                        word_array = tempword.ToCharArray();
                        word_length = word_array.Length;
                        if (word_length < 3) // checking if the word meets requirements.
                        {
                            if (word.Equals("")) {
                                continue;
                            }
                            else {
                                unfound_words.Add(word);
                            }
                            continue;
                        }
                        for (int i = 0; i < size; i++) 
                        {
                            for (int j = 0; j < size; j++) 
                            {
                                if (lettermap[i,j] == word_array[word_index])  // letter is a hit
                                {
                                    // indexes to check: (i-1, j-1), (i-1, j), (i-1,j+1), (i,j-1), (i, j+1), (i+1, j-1), (i+1, j), (i+1,j+1)

                                    // single block checking index values
                                    // 1  2  3
                                    // 4  _  5
                                    // 6  7  8
                                    // where _ is the block to check
                                    if (i == 0) { // 3 cases, top left corner, top edge, top right corner
                                        if (j == 0) { // top left corner
                                            for (int k = 0; k < 9; k++) {
                                                if (k == 1 || k == 2 || k == 3 || k == 4 || k == 6) {
                                                    continue;
                                                }
                                                else if (check_array_for_word(lettermap,word_array,i,j,k) == word_length) {
                                                    matching_row_index = i;
                                                    matching_col_index = j;
                                                    matching_start_index = k;
                                                    break;
                                                }
                                            }
                                        }
                                        else if (j == size) { // top right corner
                                            for (int k = 0; k < 9; k++) {
                                                if (k == 1 || k == 2 || k == 3 || k == 5 || k == 8) {
                                                    continue;
                                                }
                                                else if (check_array_for_word(lettermap,word_array,i,j,k) == word_length) {
                                                    matching_row_index = i;
                                                    matching_col_index = j;
                                                    matching_start_index = k;
                                                    break;
                                                }
                                            }
                                        }
                                        else { // top edge
                                            for (int k = 0; k < 9; k++) {
                                                if (k == 1 || k == 2 || k == 3) {
                                                    continue;
                                                }
                                                else if (check_array_for_word(lettermap,word_array,i,j,k) == word_length) {
                                                    matching_row_index = i;
                                                    matching_col_index = j;
                                                    matching_start_index = k;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    else if (i == size) { // 3 cases, bottom left corner, bottom edge, bottom right corner
                                        if (j == 0) { // bottom left corner
                                            for (int k = 0; k < 9; k++) {
                                                if (k == 1 || k == 4 || k == 6 || k == 7 || k == 8) {
                                                    continue;
                                                }
                                                else if (check_array_for_word(lettermap,word_array,i,j,k) == word_length) {
                                                    matching_row_index = i;
                                                    matching_col_index = j;
                                                    matching_start_index = k;
                                                    break;
                                                }
                                            }
                                        }
                                        else if (j == size) { // bottom right corner
                                            for (int k = 0; k < 9; k++) {
                                                if (k == 3 || k == 5 || k == 6 || k == 7 || k == 8) {
                                                    continue;
                                                }
                                                else if (check_array_for_word(lettermap,word_array,i,j,k) == word_length) {
                                                    matching_row_index = i;
                                                    matching_col_index = j;
                                                    matching_start_index = k;
                                                    break;
                                                }
                                            }
                                        }
                                        else { // bottom edge
                                            for (int k = 0; k < 9; k++) {
                                                if (k == 6 || k == 7 || k == 8) {
                                                    continue;
                                                }
                                                else if (check_array_for_word(lettermap,word_array,i,j,k) == word_length) {
                                                    matching_row_index = i;
                                                    matching_col_index = j;
                                                    matching_start_index = k;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    else if (j == 0 ) { // single case, left edge, not row 0 or N
                                        for (int k = 0; k < 9; k++) {
                                            if (k == 1 || k == 4 || k == 6) {
                                                continue;
                                            }
                                            else if (check_array_for_word(lettermap,word_array,i,j,k) == word_length) {
                                                matching_row_index = i;
                                                matching_col_index = j;
                                                matching_start_index = k;
                                                break;
                                            }
                                        }
                                    }
                                    else if (j == size ) { // single case, right edge, not row 0 or N
                                        for (int k = 0; k < 9; k++) {
                                            if (k == 3 || k == 5 || k == 8) {
                                                continue;
                                            }
                                            else if (check_array_for_word(lettermap,word_array,i,j,k) == word_length) {
                                                matching_row_index = i;
                                                matching_col_index = j;
                                                matching_start_index = k;
                                                break;
                                            }
                                        }
                                    }
                                    else { // regular case
                                        for (int k = 0; k < 9; k++) {
                                            if (check_array_for_word(lettermap,word_array,i,j,k) == word_length) {
                                                matching_row_index = i;
                                                matching_col_index = j;
                                                matching_start_index = k;
                                                break;
                                            }
                                        }
                                    }
                                }
                                else
                                    continue;

                                if (matching_start_index != 0)
                                    goto FINISHWORD;
                            }    
                        }
                    FINISHWORD:
                        // actual found/not found handling done here
                        if (matching_start_index != 0) { // word was found
                            // adjust the solution lettermap with the found word
                            solution_lettermap_change(solution_lettermap, word_length, matching_row_index, matching_col_index, matching_start_index);
                        }
                        else { // word was not found
                            unfound_words.Add(word);
                        }
                        matching_start_index = 0;
                    }

                    unfound_words.Sort();

                    // Exit Control and Solution/Error Log File production.
                    if (unfound_words.Count == 0)
                    {
                        if (File.Exists(solution_name)) {
                            File.Delete(solution_name);
                        }

                        // Create a file to output the solution
                        try {
                            using (StreamWriter outputfile = new StreamWriter("./" + solution_name)) {
                                outputfile.WriteLine("The lettermap solution is shown below: (in the blanks)");
                                outputfile.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                                for (int i = 0; i < size; i++) {
                                    for (int j = 0; j < size; j++) {
                                        outputfile.Write(solution_lettermap[i,j] + " ");
                                    }
                                    outputfile.Write("\n");
                                }
                            }
                        }
                        catch (IOException e)
                        {
                            Console.WriteLine(">> The solution file " + filename + " could not be produced. Reason:");
                            Console.WriteLine(e.Message);
                            Console.WriteLine("Exiting . . .");
                            return;
                        }
                        Console.WriteLine(">> Solution successfully generated. See {0}", solution_name);
                    }
                    else
                    {
                        if (File.Exists(error_log_name)) {
                            File.Delete(error_log_name);
                        }

                        // Create a file to output the solution
                        try {
                            using (StreamWriter outputfile = new StreamWriter("./" + error_log_name)) {
                                outputfile.WriteLine("The lettermap with words that could be found ");
                                outputfile.WriteLine("is shown below:");
                                outputfile.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                                for (int i = 0; i < size; i++) {
                                    for (int j = 0; j < size; j++) {
                                        outputfile.Write(solution_lettermap[i,j] + " ");
                                    }
                                    outputfile.Write("\n");
                                }
                                
                                outputfile.WriteLine(" ");
                                outputfile.WriteLine("The following words could not be found: ");
                                outputfile.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                                foreach (var word in unfound_words) {
                                    outputfile.Write(word + " ");
                                }
                            }
                        }
                        catch (IOException e)
                        {
                            Console.WriteLine(">> The error file " + filename + " could not be produced. Reason:");
                            Console.WriteLine(e.Message);
                            Console.WriteLine("Exiting . . .");
                            return;
                        }
                        Console.WriteLine(">> Solution generation fail. See {0}", error_log_name);
                    }
                    break;
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                case "help":
                    Console.WriteLine("Command \t Function \t\t\t\t\t\t Syntax");
                    char_loop_print(horizontal_line_times,'-');
                    Console.WriteLine("generate \t Generates a new crossword based on a wordlist file \t generate *size* *input filename*");
                    Console.WriteLine("solve \t\t Solves the crossword in a .txt file format \t\t solve *input filename*");
                    break;
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                default:
                    Console.WriteLine(">> That option does not exist. Type './wordsearch_gensolver help' to get the available commands");
                    break;
            }
            char_loop_print(horizontal_line_times,'~');
            
        }

        static int check_array_for_word(char?[,] lettermap, char[] word, int start_row, int start_col, int start_direction)
        {
            int letter_matches = 0;
            int word_length = word.Length;
            int size = lettermap.GetLength(0);

            // single block checking index values
            // 1  2  3
            // 4  _  5
            // 6  7  8
            // where _ is the block to check
            try {
                switch (start_direction) {
                    case 1: // top left, both row and col indices change
                        for (int i = 0; i < word_length; i++) {
                            if ((start_row - i) < 0 || (start_col - i) < 0) {
                                return letter_matches;
                            }
                            else if (lettermap[start_row - i, start_col - i] == word[i]) {
                                letter_matches++;
                            }
                            else {
                                return letter_matches;
                            }
                        }
                        break;
                    case 2: // top, only row indices change
                        for (int i = 0; i < word_length; i++) {
                            if ((start_row - i) < 0) {
                                return letter_matches;
                            }
                            else if (lettermap[start_row - i, start_col] == word[i]) {
                                letter_matches++;
                            }
                            else {
                                return letter_matches;
                            }
                        }
                        break;
                    case 3: // top right, row and col indices change
                        for (int i = 0; i < word_length; i++) {
                            if ((start_row - i) < 0|| (start_col + i) >= size) {
                                return letter_matches;
                            }
                            else if (lettermap[start_row - i, start_col + i] == word[i]) {
                                letter_matches++;
                            }
                            else {
                                return letter_matches;
                            }
                        }
                        break;
                    case 4: // left, only col indicies change
                        for (int i = 0; i < word_length; i++) {
                            if ((start_col - i) < 0) {
                                return letter_matches;
                            }
                            else if (lettermap[start_row, start_col - i] == word[i]) {
                                letter_matches++;
                            }
                            else {
                                return letter_matches;
                            }
                        }
                        break;
                    case 5: // right, only col indicies change
                        for (int i = 0; i < word_length; i++) {
                            if ((start_col + i) >= size) {
                                return letter_matches;
                            }
                            else if (lettermap[start_row, start_col + i] == word[i]) {
                                letter_matches++;
                            }
                            else {
                                return letter_matches;
                            }
                        }
                        break;
                    case 6: // bottom left, row and col indicies change
                        for (int i = 0; i < word_length; i++) {
                            if ((start_row + i) >= size || (start_col - i) < 0) {
                                return letter_matches;
                            }
                            else if (lettermap[start_row + i, start_col - i] == word[i]) {
                                letter_matches++;
                            }
                            else {
                                return letter_matches;
                            }
                        }
                        break;
                    case 7: // bottom, only row indices change
                        for (int i = 0; i < word_length; i++) {
                            if ((start_row + i) >= size) {
                                return letter_matches;
                            }
                            else if (lettermap[start_row + i, start_col] == word[i]) {
                                letter_matches++;
                            }
                            else {
                                return letter_matches;
                            }
                        }
                        break;
                    case 8: // bottom right, row and col indices change
                        for (int i = 0; i < word_length; i++) {
                            if ((start_row + i) >= size || (start_col + i) >= size) {
                                return letter_matches;
                            }
                            else if (lettermap[start_row + i, start_col + i] == word[i]) {
                                letter_matches++;
                            }
                            else {
                                return letter_matches;
                            }
                        }
                        break;
                }
            }
            catch (IndexOutOfRangeException) {
                Console.WriteLine("out of bounds {0} {1} {2} {3}",start_direction.ToString(), start_row.ToString(), start_col.ToString(), size.ToString());
                return letter_matches;
            }
            return letter_matches;
        }

        static void solution_lettermap_change(char?[,] lettermap, int word_length, int start_row, int start_col, int start_direction) {
            try {
                switch (start_direction) {
                    case 1:
                        for (int i = 0; i < word_length; i++) {
                            lettermap[start_row - i, start_col - i] = (char)32; //Char.ToLower(lettermap[start_row - i, start_col - i]);
                        }
                        break;
                    case 2:
                        for (int i = 0; i < word_length; i++) {
                            lettermap[start_row - i, start_col] = (char)32; //Char.ToLower(lettermap[start_row - i, start_col]);
                        }
                        break;
                    case 3:
                        for (int i = 0; i < word_length; i++) {
                            lettermap[start_row - i, start_col + i] = (char)32; //Char.ToLower(lettermap[start_row - i, start_col + i]);
                        }
                        break;
                    case 4:
                        for (int i = 0; i < word_length; i++) {
                            lettermap[start_row, start_col - i] = (char)32; //Char.ToLower(lettermap[start_row, start_col - i]);
                        }
                        break;
                    case 5:
                        for (int i = 0; i < word_length; i++) {
                            lettermap[start_row, start_col + i] = (char)32; //Char.ToLower(lettermap[start_row, start_col + i]);
                        }
                        break;
                    case 6:
                        for (int i = 0; i < word_length; i++) {
                            lettermap[start_row + i, start_col - i] = (char)32; //Char.ToLower(lettermap[start_row + i, start_col - i]);
                        }
                        break;
                    case 7:
                        for (int i = 0; i < word_length; i++) {
                            lettermap[start_row + i, start_col] = (char)32; //Char.ToLower(lettermap[start_row + i, start_col]);
                        }
                        break;
                    case 8:
                        for (int i = 0; i < word_length; i++) {
                            lettermap[start_row + i, start_col + i] = (char)32; //Char.ToLower(lettermap[start_row + i, start_col + i]);
                        }
                        break;
                }
            }
            catch (IndexOutOfRangeException) {
                Console.WriteLine("{0} {1} {2}", start_row, start_col, start_direction);
            }
        }

        static bool verify_lettermap_spaceforword(char?[,] lettermap, char[] word, int start_row, int start_col, int start_direction) {
            int available_blocks = 0;
            int size = lettermap.GetLength(0);

            switch (start_direction) {
                case 1:
                    for (int i = 0; i < word.Length; i++) {
                        if ((start_row - i) < 0 || (start_col - i) < 0) { // block is out of bounds, need to redo
                            return false;
                        }
                        else if (lettermap[start_row - i, start_col - i].HasValue) { // block is not empty
                            if (lettermap[start_row - i, start_col - i] == word[i]) { 
                                // block contains an identical letter to the one in the word we are placing, go with it
                                available_blocks++;
                                continue;
                            }
                            else { 
                                // block contains a letter but it is not identical, need to redo
                                return false;
                            }
                        }
                        else {
                            available_blocks++;
                        }
                    }
                    break;
                case 2:
                    for (int i = 0; i < word.Length; i++) {
                        if ((start_row - i) < 0) { // block is out of bounds, need to redo
                            return false;
                        }
                        else if (lettermap[start_row - i, start_col].HasValue) { // block is not empty
                            if (lettermap[start_row - i, start_col] == word[i]) { 
                                // block contains an identical letter to the one in the word we are placing, go with it
                                available_blocks++;
                                continue;
                            }
                            else { 
                                // block contains a letter but it is not identical, need to redo
                                return false;
                            }
                        }
                        else {
                            available_blocks++;
                        }
                    }
                    break;
                case 3:
                    for (int i = 0; i < word.Length; i++) {
                        if ((start_row - i) < 0 || (start_col + i) >= size) { // block is out of bounds, need to redo
                            return false;
                        }
                        else if (lettermap[start_row - i, start_col + i].HasValue) { // block is not empty
                            if (lettermap[start_row - i, start_col + i] == word[i]) { 
                                // block contains an identical letter to the one in the word we are placing, go with it
                                available_blocks++;
                                continue;
                            }
                            else { 
                                // block contains a letter but it is not identical, need to redo
                                return false;
                            }
                        }
                        else {
                            available_blocks++;
                        }
                    }
                    break;
                case 4:
                    for (int i = 0; i < word.Length; i++) {
                        if ((start_col - i) < 0) { // block is out of bounds, need to redo
                            return false;
                        }
                        else if (lettermap[start_row, start_col - i].HasValue) { // block is not empty
                            if (lettermap[start_row, start_col - i] == word[i]) { 
                                // block contains an identical letter to the one in the word we are placing, go with it
                                available_blocks++;
                                continue;
                            }
                            else { 
                                // block contains a letter but it is not identical, need to redo
                                return false;
                            }
                        }
                        else {
                            available_blocks++;
                        }
                    }
                    break;
                case 5:
                    for (int i = 0; i < word.Length; i++) {
                        if ((start_col + i) >= size) { // block is out of bounds, need to redo
                            return false;
                        }
                        else if (lettermap[start_row, start_col + i].HasValue) { // block is not empty
                            if (lettermap[start_row, start_col + i] == word[i]) { 
                                // block contains an identical letter to the one in the word we are placing, go with it
                                available_blocks++;
                                continue;
                            }
                            else { 
                                // block contains a letter but it is not identical, need to redo
                                return false;
                            }
                        }
                        else {
                            available_blocks++;
                        }
                    }
                    break;
                case 6:
                    for (int i = 0; i < word.Length; i++) {
                        if ((start_row + i) >= size || (start_col - i) < 0) { // block is out of bounds, need to redo
                            return false;
                        }
                        else if (lettermap[start_row + i, start_col - i].HasValue) { // block is not empty
                            if (lettermap[start_row + i, start_col - i] == word[i]) { 
                                // block contains an identical letter to the one in the word we are placing, go with it
                                available_blocks++;
                                continue;
                            }
                            else { 
                                // block contains a letter but it is not identical, need to redo
                                return false;
                            }
                        }
                        else {
                            available_blocks++;
                        }
                    }
                    break;
                case 7:
                    for (int i = 0; i < word.Length; i++) {
                        if ((start_row + i) >= size) { // block is out of bounds, need to redo
                            return false;
                        }
                        else if (lettermap[start_row + i, start_col ].HasValue) { // block is not empty
                            if (lettermap[start_row + i, start_col ] == word[i]) { 
                                // block contains an identical letter to the one in the word we are placing, go with it
                                available_blocks++;
                                continue;
                            }
                            else { 
                                // block contains a letter but it is not identical, need to redo
                                return false;
                            }
                        }
                        else {
                            available_blocks++;
                        }
                    }
                    break;
                case 8:
                    for (int i = 0; i < word.Length; i++) {
                        if ((start_row + i) >= size || (start_col + i) >= size) { // block is out of bounds, need to redo
                            return false;
                        }
                        else if (lettermap[start_row + i, start_col + i].HasValue) { // block is not empty
                            if (lettermap[start_row + i, start_col + i] == word[i]) { 
                                // block contains an identical letter to the one in the word we are placing, go with it
                                available_blocks++;
                                continue;
                            }
                            else { 
                                // block contains a letter but it is not identical, need to redo
                                return false;
                            }
                        }
                        else {
                            available_blocks++;
                        }
                    }
                    break;
            }
            if (available_blocks == word.Length) {
                return true;
            }
            else {
                return false;
            }
        }

        static void populate_lettermap_with_word(char?[,] lettermap, char[] word, int start_row, int start_col, int start_direction) {
            try {
                switch (start_direction) {
                    case 1:
                        for (int i = 0; i < word.Length; i++) {
                            lettermap[start_row - i, start_col - i] = word[i];
                        }
                        break;
                    case 2:
                        for (int i = 0; i < word.Length; i++) {
                            lettermap[start_row - i, start_col] = word[i];
                        }
                        break;
                    case 3:
                        for (int i = 0; i < word.Length; i++) {
                            lettermap[start_row - i, start_col + i] = word[i];
                        }
                        break;
                    case 4:
                        for (int i = 0; i < word.Length; i++) {
                            lettermap[start_row, start_col - i] = word[i];
                        }
                        break;
                    case 5:
                        for (int i = 0; i < word.Length; i++) {
                            lettermap[start_row, start_col + i] = word[i];
                        }
                        break;
                    case 6:
                        for (int i = 0; i < word.Length; i++) {
                            lettermap[start_row + i, start_col - i] = word[i];
                        }
                        break;
                    case 7:
                        for (int i = 0; i < word.Length; i++) {
                            lettermap[start_row + i, start_col] = word[i];
                        }
                        break;
                    case 8:
                        for (int i = 0; i < word.Length; i++) {
                            lettermap[start_row + i, start_col + i] = word[i];
                        }
                        break;
                }
            }
            catch (IndexOutOfRangeException) {
                Console.WriteLine("{0} {1} {2}", start_row, start_col, start_direction);
            }
        }

        static void print_2d_char_array(char?[,] lettermap, int size)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Console.Write(lettermap[i, j] + " ");
                }
                Console.Write("\n");
            }
        }

        static void print_1d_string_array(string[] wordlist, int size)
        {
            foreach (var word in wordlist)
            {
                Console.WriteLine(word);
            }
        }
    
        static void char_loop_print(int numtimes, char char2print) {
            for (int i = 0; i < numtimes; i++) {
                Console.Write(char2print);
            }
            Console.Write("\n");
        }

    }
}
