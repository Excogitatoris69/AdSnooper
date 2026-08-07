using CoreAdSnooper.interfaces;
using DomainAdSnooper.dto;
using System.Text;

namespace AdSnooperGui.viewmodels
{
    public class SqlQueryBasicVM : BasicVM
    {
        protected IDataService dataService { get; set; }

        protected SqlParserResultDto refreshSqlParsingResult(SqlEditorVM sqlEditorVM)
        {
            SqlParserResultDto currentPasingResult = dataService.parseSqlQueryString(sqlEditorVM.editorText);
            StringBuilder sbErrorText = new StringBuilder();
            foreach (SqlParserSyntaxErrorDto item in currentPasingResult.syntaxErrorList)
            {
                sbErrorText.Append(item.ToString());
                sbErrorText.Append("\n");
            }
            sqlEditorVM.sqlParseSyntaxErrorText = sbErrorText.ToString();
            if (currentPasingResult.syntaxErrorList.Count == 0)
            {
                sqlEditorVM.sqlParsedSyntaxValid = true;
                sqlEditorVM.sqlParsedAdQueryString = currentPasingResult.adQueryString;
            }
            else
            {
                sqlEditorVM.sqlParsedAdQueryString = "";
                sqlEditorVM.sqlParsedSyntaxValid = false;
            }
            return currentPasingResult;
        }

        
    }




}
