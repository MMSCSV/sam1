using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CareFusion.Dispensing.Contracts
{
    public partial class StandardRepeatPattern
    {
        #region Repeat Pattern Groupings

        private static IEnumerable<StandardRepeatPatternInternalCode> TimeIntervalPatterns = new StandardRepeatPatternInternalCode[]
        {
            StandardRepeatPatternInternalCode.QnS,
            StandardRepeatPatternInternalCode.QnM,
            StandardRepeatPatternInternalCode.QnH
        };

        private static IEnumerable<StandardRepeatPatternInternalCode> PRNTimeIntervalPatterns = new StandardRepeatPatternInternalCode[]
        {
            StandardRepeatPatternInternalCode.PRNQnS,
            StandardRepeatPatternInternalCode.PRNQnM,
            StandardRepeatPatternInternalCode.PRNQnH
        }; 

        private static IEnumerable<StandardRepeatPatternInternalCode> DayPatterns = new StandardRepeatPatternInternalCode[]
        {
            StandardRepeatPatternInternalCode.QnD,
            StandardRepeatPatternInternalCode.QOD,
            StandardRepeatPatternInternalCode.QnW,
            StandardRepeatPatternInternalCode.QnL,
            StandardRepeatPatternInternalCode.QnJd
        };

        private static IEnumerable<StandardRepeatPatternInternalCode> PRNPatterns = new StandardRepeatPatternInternalCode[]
        {
            StandardRepeatPatternInternalCode.PRN,
            StandardRepeatPatternInternalCode.PRNBID,
            StandardRepeatPatternInternalCode.PRNC,
            StandardRepeatPatternInternalCode.PRNQAM,
            StandardRepeatPatternInternalCode.PRNQHS,
            StandardRepeatPatternInternalCode.PRNQID,
            StandardRepeatPatternInternalCode.PRNQnD,
            StandardRepeatPatternInternalCode.PRNQnH,
            StandardRepeatPatternInternalCode.PRNQnJd,
            StandardRepeatPatternInternalCode.PRNQnL,
            StandardRepeatPatternInternalCode.PRNQnM,
            StandardRepeatPatternInternalCode.PRNQnS,
            StandardRepeatPatternInternalCode.PRNQnW,
            StandardRepeatPatternInternalCode.PRNQOD,
            StandardRepeatPatternInternalCode.PRNQPM,
            StandardRepeatPatternInternalCode.PRNQSHIFT,
            StandardRepeatPatternInternalCode.PRNTID,
            StandardRepeatPatternInternalCode.PRNxID,
        };

        private static IEnumerable<StandardRepeatPatternInternalCode> SpecificTimePattern = new StandardRepeatPatternInternalCode[]
        {
            StandardRepeatPatternInternalCode.BID,
            StandardRepeatPatternInternalCode.TID,
            StandardRepeatPatternInternalCode.QID,
            StandardRepeatPatternInternalCode.XID,
            StandardRepeatPatternInternalCode.QAM,
            StandardRepeatPatternInternalCode.QSHIFT,
            StandardRepeatPatternInternalCode.QHS,
            StandardRepeatPatternInternalCode.QPM,
            StandardRepeatPatternInternalCode.M,
            StandardRepeatPatternInternalCode.D,
            StandardRepeatPatternInternalCode.V
        };

        #endregion

        public static bool IsTimeIntervalPattern(StandardRepeatPatternInternalCode code)
        {
            return TimeIntervalPatterns.Contains(code);            
        }

        public static bool IsPRNTimeIntervalPattern(StandardRepeatPatternInternalCode code)
        {
            return PRNTimeIntervalPatterns.Contains(code);
        }

        public static bool IsPRNPattern(StandardRepeatPatternInternalCode code)
        {
            return PRNPatterns.Contains(code);
        }

        public static bool IsDayPattern(StandardRepeatPatternInternalCode code)
        {
            return DayPatterns.Contains(code);
        }

        public static bool IsSpecificTimePattern(StandardRepeatPatternInternalCode code)
        {
            return SpecificTimePattern.Contains(code);
        }
    }
}
