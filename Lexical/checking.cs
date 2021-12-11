using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;

namespace lexical_phase
{
	class Checking : Tokenizing
	{

		static IDictionary<string, string> keyword_list = new Dictionary<string, string>(){
				{"int","dt"},
				{"float","dt"},
				{"bool","dt"},
				{"str","dt"},
				{"char","dt"},
				{"main","main"},
				{"public","access-modifier"},
				{"private","access-modifier"},
				{"proteted","access-modifier"},
				{"static","static"},
				{"abstract","abstract"},
				{"sealed","sealed"},
				{"class","class"},
				{"void","void"},
				{"while","while"},
				{"if","if"},
				{"else","else"},
				{"for","for"},
				{"do","do"},
				{"break","break"},
				{"continue","continue" },
				{"return","return" },
				{"struct","struct" },
				{"new","new"},
				{"func","func"},
				 {"and","and"},
				 {"or","or"},
				 {"not","not"},

	};
		

		static List<string> Inc_Dec = new List<string> { "++", "--" };
		static List<string> MD = new List<string> { "*", "/" };
		static List<string> PM = new List<string> { "+", "-" };
		static List<string> RO = new List<string> { "<", ">", "<=", ">=", "==", "!=" };
		static List<string> comp_assign = new List<string> { "+=", "-=", "*=", "/=" };
		static List<string> assign = new List<string> { "=" };

		static Regex re_id = new Regex(@"^@?[A-Za-z]+_*[A-Za-z\d]+$");  //id

		static Regex re_int = new Regex(@"^[+-]?\d+$"); //integer constant

		static Regex re_flt = new Regex(@"^[+-]?\d*.\d+$"); //float constant

		static Regex re_char = new Regex(@"^\'(\\[\'\\\""bfnrt0]|\w|[!-/:-@[-`{-~])\'$"); //char constant

		static Regex re_str = new Regex(@"^""(\\[\'\\\""bfnrt0]|\w\s*|[!-/:-@[-`{-~]\s*)*""$"); //string constant

		public static List<Tuple< string,string,int >> tokens = new List<Tuple< string, string, int>>();



		static void Main(string[] args)
		{

			string inputtext = File.ReadAllText(@"C:\Users\HP\Desktop\houra\source.txt");
			
			break_words(inputtext);


			Lexical_Analyzer();

		}

		public static void Lexical_Analyzer()
		{
			

			String word = "";
			int line_no = 0;
			String Class_pt = "";






			foreach (var item in mylist)
			{  // item1 --->> line_no    item2 ---->> word

				word = item.Item2;
				line_no = item.Item1;

				char ch = word[0];



				if (ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z')
				{

					if (check_id(word))
					{

						Class_pt = check_kw(word);
						if (Class_pt == null)
						{

							Class_pt = "ID";
							
							
							tokens.Add(new Tuple<string, string, int>(Class_pt, word, line_no));
						}
					else if (Class_pt ==  word)
						{


						tokens.Add(new Tuple<string, string, int>(Class_pt, " ", line_no));
						}

						else
						{

							tokens.Add(new Tuple<string, string, int>(Class_pt, word , line_no));
						}
					}

					else

					{


						tokens.Add(new Tuple<string, string, int>("invalid token", word, line_no));

						
					}
				}

					else if (ch == '@')
	{

		if (check_id(word))
		{

			Class_pt = "ID";
			tokens.Add(new Tuple<string, string, int>(Class_pt, word, line_no));

		}
		else
		
		{

			tokens.Add(new Tuple<string, string, int>("invalid token", word, line_no));

		}

				}

	else if (ch == '\"')
	{

		if (check_str(word))
		{
						string temp = "";

			Class_pt = "str_const";
						for (int i = 1; i < word.Length -1; i++) { temp= temp+ word[i]; }
						word = temp;
						tokens.Add(new Tuple<string, string, int>(Class_pt, word, line_no));

					}
					else
		{

			tokens.Add(new Tuple<string, string, int>("invalid token", word, line_no));

					}
				}  
	else if (ch == '\'')
	{

		if (check_char(word))
		{

			Class_pt = "char_const";
			string temp = "";
			for (int i = 1; i < word.Length - 1; i++) { temp = temp + word[i]; }
						word = temp;
			tokens.Add(new Tuple<string, string, int>(Class_pt, word, line_no));

			}
		else
		{

			tokens.Add(new Tuple<string, string, int>("invalid token", word, line_no));

					}
				}
	else if (ch >= '0' && ch <='9')
	{

		if (check_int(word))
		{

			Class_pt = "int_const";
			tokens.Add(new Tuple<string, string, int>(Class_pt, word, line_no));

		}

		else if (check_flt(word))
		{

			Class_pt = "float_const";
			tokens.Add(new Tuple<string, string, int>(Class_pt, word, line_no));

				}

		else
		{

		 tokens.Add(new Tuple<string, string, int>("invalid token", word, line_no));

					}
				}
	else if (ch == '.')
	{


		if (check_flt(word))
		{

			Class_pt = "float_const";
		 tokens.Add(new Tuple<string, string, int>(Class_pt, word, line_no));

					}

		else if (word == ".")
		{

			tokens.Add(new Tuple<string, string, int>(".", " ", line_no));

				}

		else
		{

			tokens.Add(new Tuple<string, string, int>("invalid token", word, line_no));

					}
				}
	else if (isPunc(ch))
	{

		Class_pt = word;
		tokens.Add(new Tuple<string, string, int>(Class_pt, "", line_no));

				}


	else if (isOp(ch))
	{

		Class_pt = opclass(word);
		tokens.Add(new Tuple<string, string, int>(Class_pt, word, line_no));

				}


		else
	{

		tokens.Add(new Tuple<string, string, int>("invalid token", word, line_no));

				}


			}
			string path = @"C:\Users\HP\source\repos\lexical_phase\lexical_phase\output.txt";

			foreach (var item in tokens)
			{
			
				Console.WriteLine($"({item.Item1} ,{item.Item2} , {item.Item3} )");

				using (StreamWriter sw = File.AppendText(path))
				{
					sw.WriteLine($"({item.Item1} ,{item.Item2} , {item.Item3} )");
				}
			}
			
		
		}





		public static bool check_id(string word)
			{
			// 
			if (re_id.IsMatch(word))
				{
					return true;
				}
				else
				{
					return false;
				}

			}
			static string value;
			public static string check_kw(string word)
			{
				keyword_list.TryGetValue(word, out value);
				return value;
			}

			public static bool check_int(string word)
			{
				if (re_int.IsMatch(word))
				{
					return true;
				}
				else
				{
					return false;
				}

			}

			public static bool check_flt(string word)
			{
				if (re_flt.IsMatch(word))
				{
					return true;
				}
				else
				{
					return false;
				}

			}

		public static bool check_char(string word)
		{
			if (re_char.IsMatch(word))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
			public static bool check_str(string word)
			{
				if (re_str.IsMatch(word))
				{
					return true;
				}
				else
				{
					return false;
				}


			}

		public static string opclass(string x)
		{
			
			if(PM.Contains(x)) { return "pm"; }
			else if (Inc_Dec.Contains(x)) { return "inc_dec"; }
			else if (MD.Contains(x)) { return "md"; }
			else if (RO.Contains(x)) { return "ro"; }
			else if (comp_assign.Contains(x)) { return "comp_assign"; }
			else { return "assign"; }

		}

		
	}
}