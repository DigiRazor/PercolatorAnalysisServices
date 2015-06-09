using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Percolator.AnalysisServices
{
    internal static class Comment
    {
        internal const string PAS_HEADER = "//This query brought to you by Percolator Analysis Services.\r\n//For questions, comments, bugs, or features you would like to see, contact us from the NuGet page\r\n//at www.nuget.org/packages/PercolatorAnalysisServices or visit us at www.coopdigity.com.";
        internal const string FOR_NO_SET_NAME = "//This set was not named by the user, nor was the Tag attribute detected - a default name was provided.";
        internal const string FOR_NO_MEMBER_NAME = "//This member was not named by the user, nor was the Tag attribute detected - a default name was provided.";
        internal const string FOR_NO_SET_AXIS = "//No axis was specified for placement of this set.\r\n//Default placement for sets is the Rows(1) axis.";
        internal const string FOR_NO_MEMBER_AXIS = "//No axis was specified for placement of this member.\r\n//Default placement for members is the Columns(0) axis.";
        internal const string FOR_CREATED_REGION = "//---------- Query Scoped Calculated Members and Sets ----------";
        internal const string FOR_SELECT_REGION = "//----------             Axis selections              ----------";
        internal const string FOR_SLICER_REGION = "//----------                 Slicers                  ----------";
        internal const string FOR_FROM_REGION = "//----------             From / SubCube               ----------";
    }
}
