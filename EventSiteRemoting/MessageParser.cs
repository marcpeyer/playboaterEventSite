using System;
using System.Text;

namespace kcm.ch.EventSite.Remoting
{
	public class MessageParser
	{
		private static readonly char[] commandDelemiterChars = new char[] { ' ', '\r', '\n' };
		private readonly string rawMessage;
		private readonly string cleanMessage;
		private Operation operation = null;
		private int lastCommandPos = -1;

		public MessageParser(string message)
		{
			rawMessage = message;

			ParseCommands();

			//if operation not found set default operation and start searching for parameters
			if (operation == null)
			{
				operation = Remoting.Operation.GetDefaultOperation();
				ParseCommands();
			}

			//check if operation is defined completely
			operation.CheckParameters();

			if(lastCommandPos > -1 && rawMessage.Length > (lastCommandPos + 1))
			{
				cleanMessage = rawMessage.Substring(lastCommandPos + 1).Trim();
			}
		}

		/// <summary>
		/// Reads the next command from the rawMessage with incrementing the index.
		/// </summary>
		private string ReadNextCommand(ref int i)
		{
			int endIndex;
			string command = PeekNextCommand(i, out endIndex);
			i = endIndex + 1;
			return command;
		}

		/// <summary>
		/// Reads the next command from the rawMessage without incrementing the index.
		/// </summary>
		private string PeekNextCommand(int startIndex, out int endIndex)
		{
			int i = startIndex;
			StringBuilder sb = new StringBuilder();
			for (; i < rawMessage.Length; i++)
			{
				char c = rawMessage[i];
				if (Array.IndexOf(commandDelemiterChars, c) != -1)
				{
					if(sb.Length == 0)
					{
						continue;
					}
					break;
				}
				sb.Append(c);
			}
			//decrement i to get the last index position of the last command char
			endIndex = --i;
			return sb.Length > 0 ? sb.ToString() : null;
		}

		/// <summary>
		/// Parses all the commands to retrieve the Operation and the Parameters.
		/// </summary>
		private void ParseCommands()
		{
			int operationPos = -1;
			int i = 0;
			string command;
			while ((command = ReadNextCommand(ref i)) != null )
			{
				int j;
				bool nextCommandUsed;
				string nextCommand = PeekNextCommand(i, out j);
				bool operationComplete = HandleCommand(ref i, command, nextCommand, out nextCommandUsed, operationPos);
				if(nextCommandUsed && i != -1)
				{
					i = i + 1 + nextCommand.Length;
				}
				if(operationComplete)
				{
					return;
				}
			}
			return;
		}

		private bool HandleCommand(ref int i, string command, string nextCommand, out bool nextCommandUsed, int operationPos)
		{
			nextCommandUsed = false;
			if (operation == null)
			{
				if(Operation.TryParse(command, ref operation))
				{
					lastCommandPos = i - 1;
					operationPos = i - command.Length;
					//if operation position is not at the start of the raw message
					//begin searching for parameters at the start of the message
					if(operationPos != 0)
					{
						i = 0;
					}
					return (operation.Parameters.Length == 0);
				}
				return false;
			}
			//operationPos must be set if operation is not null so we can compare
			//this here to skip if we are again on the already handled operation pos
			if(i == operationPos)
			{
				return false;
			}

			bool allParamsSet = true;
			//parse the parameters
			foreach (Parameter parameter in operation.Parameters)
			{
				allParamsSet = HandleParameter(command, nextCommand, out nextCommandUsed, parameter, allParamsSet, i);
			}
			
			return allParamsSet;
		}

		private bool HandleParameter(string command, string nextCommand, out bool nextCommandUsed, Parameter parameter, bool allParamsSet, int i)
		{
			nextCommandUsed = false;
			if (parameter is ParameterOrGroup)
			{
				allParamsSet = HandleParameterGroup(parameter, command, nextCommand, out nextCommandUsed, allParamsSet, i);
			}
			else
			{
				//pass nextCommand to tryset because if it's a PrefixParameter and a space is given after the prefix.
				if (!parameter.IsSet)
				{
					if (parameter.TrySet(command, nextCommand, out nextCommandUsed))
					{
						lastCommandPos = lastCommandPos > i ? lastCommandPos : i;
					}
					else
					{
						allParamsSet = false;
					}
				}
			}
			return allParamsSet;
		}

		private bool HandleParameterGroup(Parameter parameter, string command, string nextCommand, out bool nextCommandUsed, bool allParamsSet, int i)
		{
			nextCommandUsed = false;
			ParameterOrGroup orParameter = (ParameterOrGroup)parameter;
			foreach (Parameter p in orParameter.Parameters)
			{
				if (p is ParameterOrGroup)
				{
					allParamsSet = HandleParameterGroup(p, command, nextCommand, out nextCommandUsed, allParamsSet, i);
				}
				else
				{
					if (!p.IsSet)
					{
						if (p.TrySet(command, nextCommand, out nextCommandUsed))
						{
							lastCommandPos = lastCommandPos > i ? lastCommandPos : i;
							break;
						}
					}
				}
			}
			if (!orParameter.IsSet || !orParameter.IsValid)
			{
				allParamsSet = false;
			}
			return allParamsSet;
		}

		public Operation Operation
		{
			get
			{
				return operation;
			}
		}

		public string CleanMessage
		{
			get
			{
				return cleanMessage;
			}
		}
	}
}