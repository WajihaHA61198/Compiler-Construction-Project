using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using System.Text;


//implement line change as word breaker
namespace lexical_analyzer
{
    class Program
    {
        static IDictionary<char, string> local_symbols = new Dictionary<char, string>(){

          {'(', "("},
          {')', ")"},
          {'{', "{"},
          {'}', "}"},
          {'[', "["},
          {']', "]"},
          {'#', "commentor"},
          {'@', "lexical_error"},
          {'$', "lexical_error"},
          {'^', "lexical_error"},
          {'`', "lexical_error"},
          {'~', "lexical_error"},
          {';',";"},
          {':',":"},
          {',',","},
          {'?',"lexical_error"},
          {'\\',"lexical_error"},
          {'/',"lexical_error"}
        };

        static IDictionary<string, string> keyword_list = new Dictionary<string, string>(){
                {"int","dt"},
                {"float","dt"},
                {"bool","dt"},
                {"string","dt"},
                {"char","dt"},
                {"List","list"},
                {"public","access-modifier"},
                {"private","access-modifier"},
                {"static","static"},
                {"abstract","abstract"},
                {"var","var"},
                {"class","class"},
                {"void","void"},
                {"while","while"},
                {"if","if"},
                {"else","else"},
                {"for","for"},
                {"do","do"},
                {"break","break"},
                {"true","bool-const"},
                {"false","bool-const"},
                {"include","include"},


            };


        static int line_number = 1;
        static Regex RE_integer = new Regex(@"^\d*$");

        static Regex RE_alphabtes = new Regex(@"^[A-Za-z]$");
        static Regex RE_number = new Regex(@"^[0-9]$");
        static Regex RE_float = new Regex(@"^(\d*.\d+|\d*[^.])$");
        static Regex RE_punctuators = new Regex(@"^,|.|;|[|]|(|)|{|}|:$");
        static Regex RE_all_punctuators = new Regex(@"^[\x20-\x2F]|[\x3A-\x40]|[\x5B-\x5E]|[\x7B-\x7E]|`$");
        static Regex RE_identifier = new Regex(@"^([a-zA-Z]+_*[0-9]*)$");
        static bool line_increase(ref int index)
        {
            line_number++;
            index++;
            return true;
        }
        static bool generate_error(string value)
        {
            string path = "./tokens.txt";
            string content = "(" + value + " , lexical error" + " , " + line_number + " )" + Environment.NewLine;
            // Console.WriteLine(content);

            try
            {
                File.AppendAllText(path, content);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        static bool generate_token(string value, string class_)
        {
            string path = "./tokens.txt";
            string content = "( " + value + " , " + class_ + " , " + line_number + " )" + Environment.NewLine;

            // Console.WriteLine(content);
            try
            {
                File.AppendAllText(path, content);
                File.AppendAllText("./tokens_for_syntex.txt", class_ + ',');
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        static bool number_analyzer(ref string input, ref int index)
        {
            //assuming index is before starting letter of token
            string temp = "";
            bool error_flag = false;
            temp += input[index];

            while (true)
            {
                //in case of simple number
                if (RE_number.IsMatch(input[index + 1].ToString()))
                {
                    index++;
                    temp += input[index];
                }
                else if (input[index + 1] == '.')
                {

                    //incase of float
                    if (RE_number.IsMatch(input[index + 2].ToString()))
                    {
                        index += 1;
                        temp += input[index];
                        index += 1;
                        temp += input[index];
                        while (true)
                        {
                            if (RE_number.IsMatch(input[index + 1].ToString()))
                            {
                                temp += input[index];
                                index++;
                            }
                            // here should be better termination
                            else if (RE_alphabtes.IsMatch(input[index + 1].ToString()))
                            {
                                // generate_token(temp, "float");
                                temp += input[index + 1];
                                index++;
                                error_flag = true;
                                // generate_token(temp, "float");
                                // return true;
                            }
                            else
                            {
                                if (error_flag)
                                {
                                    index++;
                                    generate_error(temp);
                                    return false;
                                }
                                else
                                {
                                    index++;
                                    generate_token(temp, "float-const");
                                    return true;
                                }

                            }
                        }
                    }
                    else
                    {
                        index += 1;
                        temp += input[index];
                        index++;
                        generate_error(temp);
                        return false;
                    }
                }
                else if (RE_alphabtes.IsMatch(input[index + 1].ToString()))
                {

                    index++;
                    temp += input[index];
                    error_flag = true;
                    // generate_token(temp, "int-const");
                    // return true;
                }
                else
                {
                    index++;
                    if (error_flag)
                    {
                        generate_error(temp);
                        return true;
                    }
                    else
                    {
                        generate_token(temp, "int-const");
                        return true;
                    }
                }
            }
        }

        static bool punctuators(ref string input, ref int index)
        {
            string result;
            string temp = "";
            if (input[index] == '+')
            {
                if (input[index + 1] == '+')
                {
                    index += 2;
                    generate_token("++", "inc-dec");
                    return true;
                }
                // else if (input[index + 1] == '=')
                // {
                //     index += 2;
                //     generate_token("+=", "add_assign");
                //     return true;

                // }
                else
                {
                    index++;
                    generate_token("+", "AO");
                    return true;
                }
            }
            else if (input[index] == '-')
            {
                if (input[index + 1] == '-')
                {
                    index += 2;
                    generate_token("--", "inc-dec");
                    return true;
                }
                // else if (input[index + 1] == '=')
                // {
                //     index += 2;
                //     generate_token("-=", "arthmetic_assign");
                //     return true;
                // }
                else
                {
                    index += 1;
                    generate_token("-", "AO");
                    return true;
                }

            }
            else if (input[index] == '*' || input[index] == '/' || input[index] == '%')
            {

                // if (input[index + 1] == '=')
                // {
                //     temp += input[index];
                //     index++;
                //     temp += input[index];
                //     index++;

                //     generate_token(temp, "arthmetic_assign");
                //     return true;
                // }
                // else
                {
                    generate_token(input[index].ToString(), "AO");
                    index++;
                    return true;
                }
            }
            else if (input[index] == '&')
            {
                if (input[index + 1] == '&')
                {
                    index += 2;
                    generate_token("&&", "LO");
                    return true;
                }
                else
                {
                    index++;
                    generate_error("&");
                    return false;
                }
            }
            else if (input[index] == '|')
            {
                if (input[index + 1] == '|')
                {
                    index += 2;
                    generate_token("||", "LO");
                    return true;
                }
                else
                {
                    index++;
                    generate_error("|");
                    return false;
                }
            }
            else if (input[index] == '<' || input[index] == '>')
            {
                if (input[index + 1] == '=')
                {
                    generate_token(input[index] + "=", "RO");
                    index += 2;
                    return true;
                }
                else
                {
                    generate_token(input[index].ToString(), "RO");
                    index++;
                    return true;
                }
            }
            else if (input[index] == '!')
            {
                if (input[index + 1] == '=')
                {
                    index += 2;
                    generate_token("!=", "RO");
                    return true;
                }
                else
                {
                    index++;
                    generate_token("!", "not");
                    return true;

                }
            }
            else if (input[index] == '=')
            {
                if (input[index + 1] == '=')
                {
                    index += 2;
                    generate_token("==", "RO");
                    return true;
                }
                else
                {
                    index++;
                    generate_token("=", "equal");
                    return true;
                }

            }
            else if (input[index] == '.')
            {
                //probaly its a float
                if (RE_number.IsMatch(input[index + 1].ToString()))
                {
                    temp = "";
                    bool error_flag = false;
                    // . got inside
                    temp += input[index];
                    index++;
                    //first number after dot got inside
                    temp += input[index];
                    index++;

                    while (true)
                    {
                        if (RE_number.IsMatch(input[index].ToString()))
                        {
                            temp += input[index];
                            index++;
                        }
                        else if (RE_alphabtes.IsMatch(input[index].ToString()))
                        {
                            temp += input[index];
                            index++;
                            error_flag = true;
                        }
                        else
                        {
                            //token or error
                            if (error_flag)
                            {
                                generate_error(temp);
                                return false;
                            }
                            else
                            {
                                generate_token(temp, "float-const");
                                return true;
                            }
                        }
                    }


                }
                // else just a dot
                else
                {
                    index++;
                    generate_token(".", "dot");
                    return true;
                }
            }
            // incase local symbols
            else if (local_symbols.TryGetValue(input[index], out result))
            {
                generate_token(input[index].ToString(), result);
                index++;
                return true;
            }
            //incase white space
            else if (input[index] == ' ' || input[index] == '\t')
            {
                //ignore
                index++;
                return true;
            }
            else if (input[index] == '\r')
            {
                if (input[index + 1] == '\n')
                {
                    // System.Console.WriteLine("test1");
                    index++;
                    line_increase(ref index);
                    return true;
                }
                else
                {
                    index++;
                    return true;

                }
            }
            else if (input[index] == '\n')
            {
                line_increase(ref index);
                return true;
            }
            else
            {
                // System.Console.WriteLine((int)input[index]);
                generate_error(input[index].ToString());
                index++;
            }
            return false;
        }
        static bool keyword_RE_checker(ref string temp, out string type)
        {
            if (keyword_list.TryGetValue(temp, out type))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        static bool identifier_RE_checker(ref string temp, out string type)
        {
            if (RE_identifier.IsMatch(temp))
            {
                type = "ID";
                return true;
            }
            else
            {
                type = "";
                return false;
            }
        }
        static bool alphabet_analyzer(ref string input, ref int index)
        {
            string type;
            string temp = "";
            temp += input[index];

            while (true)
            {

                if (RE_alphabtes.IsMatch(input[index + 1].ToString()))
                {
                    index++;
                    temp += input[index];
                }
                else
                {
                    // if keyword matched then just generate token
                    if (keyword_RE_checker(ref temp, out type))
                    {
                        // generate token
                        generate_token(temp, type);
                        index++;
                        return true;
                    }
                    // otherwise check for other possibilites
                    // i.e identifier or error
                    else
                    {
                        // Encoding.ASCII.
                        // char.IsWhiteSpace(c)
                        while (true)
                        {
                            // index++;
                            if (input[index + 1] == '_' || ((int)input[index + 1] >= 48 && (int)input[index + 1] <= 57))
                            {
                                temp += input[index + 1];
                                index++;
                            }
                            else
                            {
                                if (identifier_RE_checker(ref temp, out type))
                                {
                                    index++;
                                    generate_token(temp, type);
                                    return true;
                                }
                                else
                                {
                                    index++;
                                    generate_error(temp);
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
        }
        //=======================================================================================

        // ---single quote -> '', for char---
        static bool single_quote(ref string input, ref int index)
        {
            string temp = "";
            bool back_slash_flag = false;   //b_slash  //back_slash_flag
            bool error_flag = false;

            //got first quote inside
            temp += input[index];
            //temp = temp + input[index];
            index++;
            char[] escape_sequence = { '0', '\'', '\"', '\\', 'b', 'n', 't', 'f', 'v' };   //esc_Seq

            while (input[index] != '\n')  //till next line
            {
                //got back slash turn on the flag
                if (input[index] == '\\' && input[index - 1] != '\\') //not comment
                {
                    back_slash_flag = true;
                    error_flag = true;

                    for (int i=0; i<escape_sequence.Length; i++)
                    {
                        try
                        {
                            if (input[index+1] == escape_sequence[i])
                            {
                                error_flag = false;
                                break;
                            }
                        }
                        catch
                        {
                        }
                    }
                }//

                //got ending quote
                if (input[index] == '\'')
                {
                    if (input[index-1] == '\\' && input[index-2] != '\\')  // check for comment, it must not, just single -> \
                    {
                        temp += input[index];
                        //temp = temp +input[index];    
                        index++;
                    }
                    //other wise end it error or successful token
                    else
                    {
                        temp += input[index];
                        //temp = temp +input[index]; 
                        // Console.WriteLine(temp);
                        index++;

                        // incase wrong escape sequence
                        if (error_flag)
                        {
                            generate_error(temp);
                            return false;
                        }
                        if (back_slash_flag)  // if back slash found, allow only 4 bit
                        {
                            if (temp.Length == 4)
                            {
                                generate_token(temp, "char-const");
                                return true;
                            }
                            else //invalid token
                            {
                                generate_error(temp);
                                return false;
                            }
                        }
                        else
                        {
                            if (temp.Length == 3) //otherwise 3 bit for char
                            {
                                generate_token(temp, "char-const");
                                return true;
                            }
                            else
                            {
                                generate_error(temp);
                                return false;
                            }
                        }
                    }
                }// if
                //simple going on
                else
                {
                    temp += input[index];
                    //temp = temp +input[index]; 
                    // Console.WriteLine(temp);
                    index++;
                }
            } // while loop end
            //means char ended due to line change char therefore error
            generate_error(temp);
            //here mean ended at /n
            return false;
        }
        //=======================================================================================

        // ---double quote -> "", for string---
        static bool double_quote(ref string input, ref int index)
        {
            string temp = "";
            temp += input[index];  //temp = temp + input[index;]
            index++;

            while (index<input.Length)   //read till end
            {
                if (input[index] == '\r')   //"---------------/r -> Invalid token
                {
                    index++;
                    generate_error(temp);
                    line_increase(ref index);
                    return false;
                }//
                if (input[index] == '\"')  //"---------------" -> found
                {
                    if (input[index-1] == '\\')  //check if in previous index isn't \, if yes than next char(") ignore
                    {
                        temp += input[index];
                        //temp = temp + input[index];
                        temp += input[index + 1];
                        index += 2;
                    }
                    else
                    {
                        //generate token
                        temp += input[index];
                        //temp = temp + input[index];
                        index++;
                        generate_token(temp, "string-const");
                        return true;
                    }
                }
                else
                {
                    temp += input[index];
                    //temp = temp + input[index];
                    index++;
                }//
            }// while loop end
            return false;
        }
        //=======================================================================================

        // ---SINGLE-LINE COMMENT---
        static bool SL_comment(ref string input, ref int index)
        {
            while (index<input.Length)
            {
                if (input[index] == '\r')  //carriage return is comment breaker
                {
                    index++;
                    line_increase(ref index);
                    return true;
                }
                index++;
            }// whlie loope end
            return true;
        }
        //=======================================================================================

        // ---MULTI-LINE COMMENT---
        static bool ML_comment(ref string input, ref int index)
        {
            while (index<input.Length)   //when to end comment
            {
                if (input[index] == '\r')  //carriage return -> \r (13)
                {
                    index++;
                    line_increase(ref index);
                }
                if (input[index] == '*')
                {
                    if (input[index + 1] == '/')
                    {
                        index = index + 2;
                        return true;
                    }
                }
                index++;
            } // while loop end
            return true;
        }
        //=======================================================================================

        // ---MAIN---
        static void Main(string[] args)
        {
            // Connecting source file 
            string source_file = File.ReadAllText("./Input.txt");
            source_file += " ";                               //read char-by-char
            string path = "./tokens.txt";
            File.WriteAllText(path, "");                      //token destination
            //File.WriteAllText("./T_Syntax.txt", "");        //tokens_for_syntex

            int i=0;
            while (i < source_file.Length)                    //Read till end of source file
            {
                // Comments
                if (source_file[i] == '/')
                {
                    // single-line comment
                    if (source_file[i + 1] == '/')
                    {
                        i = i + 2;  //i +=2
                        SL_comment(ref source_file, ref i);  
                    }
                    // multiline comment
                    if (source_file[i + 1] == '*')
                    {
                        System.Console.WriteLine("test");

                        i = i + 2;  
                        ML_comment(ref source_file, ref i);
                    }
                }//

                // Single quote-> ''  double quote-> ""
                if (source_file[i] != '\'' && source_file[i] != '\"')
                {
                    //start with alpha/ kW/ ID/ or Invalid
                    if (RE_alphabtes.IsMatch(source_file[i].ToString()))          //start with alpha/ KW
                    {
                        alphabet_analyzer(ref source_file, ref i);
                    }
                    else if (RE_number.IsMatch(source_file[i].ToString()))        //start with digit
                    {
                        number_analyzer(ref source_file, ref i);
                    }
                    else if (RE_punctuators.IsMatch(source_file[i].ToString()))   //symbols/ punctuator
                    {
                        punctuators(ref source_file, ref i);
                    }
                }
                else if (source_file[i] == '\'')  //single quote-> '' , for char
                {
                    single_quote(ref source_file, ref i);
                }
                else if (source_file[i] == '\"')  //double quote-> "" , for string
                {
                    double_quote(ref source_file, ref i);
                }
            }//end of while loop
        }//=======================================================================================================
    }
}
