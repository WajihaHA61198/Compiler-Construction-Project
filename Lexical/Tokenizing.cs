using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace lexical_phase
{
	class Tokenizing
	{
		static char[] op = { '+', '-', '*', '/', '<', '>', '=', '!', '&', '|' };
		static char[] punc = { '.', ',', ':', '{', '}', '(', ')', ';' };
		public static List<Tuple<int, string>> mylist = new List<Tuple<int, string>>();


		/*	 static void Main(string[] args)
			{



				

				string inputtext = File.ReadAllText(@"C:\Users\HP\Desktop\houra\source.txt");

				break_words(inputtext);

			foreach (var item in mylist)
				{
					Console.Write("line no is = "+item.Item1.ToString() +" ");
					Console.WriteLine("word is yeh = "+item.Item2);
				}

			

		}*/


		public static void break_words(string inputtext)
		{


			int line_count = 1;
			string temp_word = null;

			for (int i = 0; i < inputtext.Length; i++)
			{
			//	char temp = inputtext[i];

				if (inputtext[i] == '\'')
				{

					if (inputtext[i + 1] == '\\')
					{
						for (int j = 0; j < 4; j++)
						{
							temp_word = temp_word + inputtext[i + j];
						}
						//word.Add(temp_word);

						mylist.Add(new Tuple<int, string>(line_count, temp_word));
						//i = i + 4;
						i = i + 3;
					}
					else
					{
						for (int j = 0; j < 3; j++)
						{
							temp_word = temp_word + inputtext[i + j];

						}
						//	word.Add(temp_word);
						// add an item
						mylist.Add(new Tuple<int, string>(line_count, temp_word));
						i = i + 2;
					}
					temp_word = null;
				}
				else if (inputtext[i] == '\"')
				{
					//temp_word = null;
					//temp_word = temp_word + temp;

					while (i < inputtext.Length)
					{
						if (inputtext[i + 1] == '\r')
						{
							temp_word = temp_word + inputtext[i];
							mylist.Add(new Tuple<int, string>(line_count, temp_word));

							line_count++;
							break;

						}
						else if (inputtext[i + 1] == '\"')
						{
							temp_word = temp_word + inputtext[i];
							temp_word = temp_word + inputtext[i + 1];

							break;

						}
						else
						{
							temp_word = temp_word + inputtext[i];

						}

						i++;
					}
					//	word.Add(temp_word);
					// add an item
					if (inputtext[i + 1] != '\r')
					{
						mylist.Add(new Tuple<int, string>(line_count, temp_word));

					}
					i++;
					temp_word = null;
				}

				else if (inputtext[i] == ' ' || inputtext[i] == '\t' || inputtext[i] == '\n' || inputtext[i] == '\r' || isPunc(inputtext[i]))
				{

					int flag = 0;
					if (inputtext[i] == '\r')
					{
						if (temp_word != null)
						{

							mylist.Add(new Tuple<int, string>(line_count, temp_word));
							temp_word = null;
							//	line_count++;
							//temp = '\0';

						}
						line_count++;

					}

					else if (isPunc(inputtext[i]))
					{

						if (inputtext[i] == '.')
						{
							if (temp_word != null)
							{
								for (int j = 0; j < temp_word.Length; j++)
								{
									if ((temp_word[j] >= '0' && temp_word[j] <= '9') && temp_word[j] != '.')
									{
										flag = 1;

									}
									else
									{
										flag = 0;
										break;
									}
								}
								if (flag == 1 && (inputtext[i + 1] >= '0' && inputtext[i + 1] <= '9'))
								{
									temp_word = temp_word + inputtext[i];
									temp_word = temp_word + inputtext[i + 1];
									i++;

								}
								else
								{

									//word.Add(temp_word);
									//word.Add(temp.ToString());
									// add an item
									mylist.Add(new Tuple<int, string>(line_count, temp_word));
									temp_word = null;
									i--;
								}
							}

							else
							{

								if ((inputtext[i + 1] >= '0' && inputtext[i + 1] <= '9'))
								{
									temp_word = temp_word + inputtext[i];
									temp_word = temp_word + inputtext[i + 1];
									i++;
								}
								else
								{
									mylist.Add(new Tuple<int, string>(line_count, inputtext[i].ToString()));
								}

							}



						}
						else
						{
							if (temp_word != null)
							{
								//word.Add(temp_word);
								mylist.Add(new Tuple<int, string>(line_count, temp_word));
							}
							// add an item
							mylist.Add(new Tuple<int, string>(line_count, inputtext[i].ToString()));

							temp_word = null;
						}

					}

					else if (temp_word != null)
					{
						//word.Add(temp_word);
						// add an item
						mylist.Add(new Tuple<int, string>(line_count, temp_word));

						temp_word = null;

					}

				}

				else if (isOp(inputtext[i]))
				{


					if (temp_word != null)
					{
						//word.Add(temp_word);
						// add an item
						mylist.Add(new Tuple<int, string>(line_count, temp_word));
						temp_word = null;

					}


					if ((inputtext[i] == '&' && inputtext[i + 1] == '&') || (inputtext[i] == '|' && inputtext[i + 1] == '|'))
					{    // logical op
						temp_word = inputtext[i] + inputtext[i + 1].ToString();
						//word.Add(temp_word);
						i++;

					}
					else if ((inputtext[i] == '+' && inputtext[i + 1] == '+') || (inputtext[i] == '-' && inputtext[i + 1] == '-'))
					{     // incre decre op
						temp_word = inputtext[i] + inputtext[i + 1].ToString();

						i++;
					}
					else if ((inputtext[i] == '+' || inputtext[i] == '-' || inputtext[i] == '*' || inputtext[i] == '/' || inputtext[i] == '>' || inputtext[i] == '<' || inputtext[i] == '=' || inputtext[i] == '!') && (inputtext[i + 1] == '='))
					{     // comp op
						temp_word = inputtext[i] + inputtext[i + 1].ToString();
						//mylist.Add(new Tuple<int, string>(line_count, temp_word));

						//word.Add(temp_word);
						i++;
					}

					else
					{
						//	word.Add(temp.ToString());
						// add an item
						mylist.Add(new Tuple<int, string>(line_count, inputtext[i].ToString()));

						temp_word = null;

					}

					//temp_word = null;
				}
				// single line comment
				else if (inputtext[i] == '#' && inputtext[i + 1] == '#')
				{
					i += 2;
					while (inputtext[i] != '\r')
					{

						if (inputtext[i + 1] == '\r') line_count++;
						i++;
					}

				}

				//multi line comment
				else if (inputtext[i] == '~' && inputtext[i + 1] == '~' && inputtext[i + 2] == '~')
				{
					bool check = false;

					i += 3;

					while (inputtext[i] != '~')
					{

						if (inputtext[i] == '\r')
						{
							line_count++;
							i++;

						}
						if (inputtext[i + 1] == '~' && inputtext[i + 2] == '~' && inputtext[i + 3] == '~')
						{

							i += 4;
							break;
						}

						else
						{
							i++;
						}

					}
				}

				else
				{
					temp_word = temp_word + inputtext[i];
					
				}
			}

		}

		

		public static bool isPunc(char x)
		{
			bool check = false;
			foreach (var y in punc)
			{
				if (y == x)
				{
					check = true;
				}
			}
			return check;
		}
		public static bool isOp(char x)
		{
			bool check = false;
			foreach (var y in op)
			{
				if (y == x)
				{
					check = true;
				}
			}
			return check;
		}

	


}
	
}

