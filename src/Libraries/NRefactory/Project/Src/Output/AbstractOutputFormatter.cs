/*
 * Created by SharpDevelop.
 * User: Omnibrain
 * Date: 15.09.2004
 * Time: 10:48
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Text;
using System.Collections;
using System.Diagnostics;

using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.CSharp;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.PrettyPrinter
{
	/// <summary>
	/// Base class of output formatters.
	/// </summary>
	public abstract class AbstractOutputFormatter
	{
		int           indentationLevel = 0;
		StringBuilder text = new StringBuilder();
		
		bool          indent         = true;
		bool          doNewLine      = true;
		AbstractPrettyPrintOptions prettyPrintOptions;
		
		public int IndentationLevel {
			get {
				return indentationLevel;
			}
			set {
				indentationLevel = value;
			}
		}
		
		public string Text {
			get {
				return text.ToString();
			}
		}
		
		
		public bool DoIndent {
			get {
				return indent;
			}
			set {
				indent = value;
			}
		}
		
		public bool DoNewLine {
			get {
				return doNewLine;
			}
			set {
				doNewLine = value;
			}
		}
		
		public AbstractOutputFormatter(AbstractPrettyPrintOptions prettyPrintOptions)
		{
			this.prettyPrintOptions = prettyPrintOptions;
		}
		
		public void Indent()
		{
			if (DoIndent) {
				int indent = 0;
				while (indent < prettyPrintOptions.IndentSize * indentationLevel) {
					char ch = prettyPrintOptions.IndentationChar;
					if (ch == '\t' && indent + prettyPrintOptions.TabSize > prettyPrintOptions.IndentSize * indentationLevel) {
						ch = ' ';
					}
					text.Append(ch);
					if (ch == '\t') {
						indent += prettyPrintOptions.TabSize;
					} else {
						++indent;
					}
				}
			}
		}
		
		public void Space()
		{
			text.Append(' ');
		}
		
		int lastLineStart = 0;
		
		public virtual void NewLine()
		{
			if (DoNewLine) {
				text.AppendLine();
				lastLineStart = text.Length;
			}
		}
		
		public virtual void EndFile()
		{
		}
		
		protected void WriteInPreviousLine(string txt)
		{
			if (text.Length == lastLineStart) {
				Indent();
				text.AppendLine(txt);
				lastLineStart = text.Length;
			} else {
				string lastLine = text.ToString(lastLineStart, text.Length - lastLineStart);
				text.Remove(lastLineStart, text.Length - lastLineStart);
				Indent();
				text.AppendLine(txt);
				lastLineStart = text.Length;
				text.Append(lastLine);
			}
		}
		
		public void PrintTokenList(ArrayList tokenList)
		{
			foreach (int token in tokenList) {
				PrintToken(token);
				Space();
			}
		}
		
		public abstract void PrintComment(Comment comment);
		
		public virtual void PrintPreProcessingDirective(PreProcessingDirective directive)
		{
			WriteInPreviousLine(directive.Cmd + " " + directive.Arg);
		}
		
		public abstract void PrintToken(int token);
		
		protected void PrintToken(string text)
		{
			this.text.Append(text);
		}
		
		public void PrintIdentifier(string identifier)
		{
			text.Append(identifier);
		}
	}
}
