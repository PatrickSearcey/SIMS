<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TCPPrint.aspx.cs" Inherits="Safety.TCPPrint" %>
<%@ Register Src="~/Control/SitePageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SIMS - Traffic Control Plan</title>
    <meta name="viewport" content="initial-scale=1.0, minimum-scale=1, maximum-scale=1.0, user-scalable=no" />
    <link href="styles/base.css" rel="stylesheet" />
    <link href="styles/reports.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
    <telerik:RadScriptManager runat="server"></telerik:RadScriptManager>
    <telerik:RadFormDecorator RenderMode="Lightweight" runat="server" DecoratedControls="All" Skin="Bootstrap" />
    <div id="wrapper" class="PrintView">
        <uc:PageHeading id="ph1" runat="server" />

        <div class="mainContentTCP">
            <table>
                <tr>
                    <td width="70%">
                        <asp:Panel ID="pnlPlan0" runat="server" CssClass="pnlSiteInfoPrint" ClientIDMode="Static">
                            <h4>Site and Plan Info</h4>
                            <b>Plan Updated: </b><asp:Literal ID="ltlUpdated0" runat="server" /><br /> 
                            <b>Site Specific Notes - Reason No TCP is Required:</b><br />
                            <asp:Literal ID="ltlNotes0" runat="server" />
                        </asp:Panel>

                        <asp:Panel ID="pnlPlanV" runat="server" CssClass="pnlSiteInfoPrint" ClientIDMode="Static">
                            <h4>Site and Plan Info</h4>
                            <b>Highway/Road: </b><asp:Literal ID="ltlHighwayV" runat="server" /><br />
                            <b>Speed Limit: </b><asp:Literal ID="ltlSpeedV" runat="server" /><br /> 
                            <b>Traffic Volume: </b><asp:Literal ID="ltlTrafficV" runat="server" /><br />
                            <b>Plan Updated: </b><asp:Literal ID="ltlUpdatedV" runat="server" /><br /> 
                            <b>Plan Reviewed: </b><asp:Literal ID="ltlReviewedV" runat="server" /><br />
                            <b>Plan Approved: </b><asp:Literal ID="ltlApprovedV" runat="server" /><br /> 
                            <b>Site Specific Notes:</b><br />
                            <asp:Literal ID="ltlNotesV" runat="server" />
                            <br />
                            <b>Plan Specific Remarks:</b><br />
                            <asp:Literal ID="ltlRemarksV" runat="server" /><br />
                            <b>Plan is for daylight hours ONLY.</b>
                        </asp:Panel>

                        <asp:Panel ID="pnlSiteInfo" runat="server" CssClass="pnlSiteInfoPrint" ClientIDMode="Static">
                            <h4>Site and Plan Info</h4>
                            <b>Highway/Road: </b><asp:Literal ID="ltlHighway" runat="server" /><br />
                            <b>Freeway or Expressway: </b><asp:Literal ID="ltlFreeway" runat="server" /><br />
                            <b>Bridge Width: </b><asp:Literal ID="ltlWidth" runat="server" /><br />
                            <b>Bridge Length: </b><asp:Literal ID="ltlWorkZone" runat="server" /><br />
                            <b>Lane Width: </b><asp:Literal ID="ltlLane" runat="server" /><br />
                            <b>Shoulder Width: </b><asp:Literal ID="ltlShoulder" runat="server" /><br />
                            <b>Speed Limit: </b><asp:Literal ID="ltlSpeed" runat="server" /><br /> 
                            <b>Traffic Volume: </b><asp:Literal ID="ltlTraffic" runat="server" /><br />
                            <b>Cell Service Available: </b><asp:Literal ID="ltlCell" runat="server" /><br />
                            <b>Plan Updated: </b><asp:Literal ID="ltlUpdated" runat="server" /><br /> 
                            <b>Plan Reviewed: </b><asp:Literal ID="ltlReviewed" runat="server" /><br />
                            <b>Plan Approved: </b><asp:Literal ID="ltlApproved" runat="server" /><br />           
                            <b>Site Specific Notes:</b><br />
                            <asp:Literal ID="ltlNotes" runat="server" />
                            <br />
                            <b>Plan Specific Remarks:</b><br />
                            <asp:Literal ID="ltlRemarks" runat="server" /><br />
                            <b>Plan is for daylight hours ONLY.</b>
                        </asp:Panel>
                    </td>
                    <td valign="top">
                        <asp:Panel ID="pnlInstructions" runat="server" CssClass="pnlInstructionsPrint" ClientIDMode="Static">
                            <h4>Instructions</h4>
                            <asp:Literal ID="ltlInstructions" runat="server" />
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Panel ID="pnlAllData" runat="server" CssClass="pnlCalcDataPrint" ClientIDMode="Static">
                            <h4>TCP Calculator Data</h4>
                            <b style='color:red;'>WS* &mdash; </b><b>Advanced</b> <b style='color:red;'> Warning Sign </b><b>Spacing per sign:</b> <asp:Literal ID="ltlWS" runat="server" /> feet<br />
                            <b style='color:grey;'>F &mdash; Flagger</b> <b>Distance from the shoulder taper:</b> <asp:Literal ID="ltlFlagger" runat="server" /> feet<br />
                            <b style='color:orange;'>WZ** &mdash; Work Zone </b><b>Length**:</b> <asp:Literal ID="ltlWZLength" runat="server" /> feet<br />
                            <b style='color:orange;'>WZ** &mdash; </b><b>Number of cones in the</b> <b style='color:orange;'>Work Zone:</b> <asp:Literal ID="ltlWZCones" runat="server" /><br />
                            <b style='color:orange;'>WZ** &mdash; Work Zone </b><b>cone spacing:</b> <asp:Literal ID="ltlWZConeSpacing" runat="server" /> feet<br />
                            <b style='color:mediumpurple;'>T &mdash; </b><b>Minimum </b><b style='color:mediumpurple';>Transition Taper length:</b> <asp:Literal ID="ltlTTLength" runat="server" /> feet<br />
                            <b style='color:mediumpurple;'>T &mdash; </b><b>Number of cones in the </b><b style='color:mediumpurple';>Transition Taper:</b> <asp:Literal ID="ltlTTCones" runat="server" /><br />
                            <b style='color:mediumpurple;'>T &mdash; </b><b>Maximum spacing for cones in the </b><b style='color:mediumpurple';>Transition Taper:</b> <asp:Literal ID="ltlTTConeSpacing" runat="server" /> feet<br />
                            <b style='color:cornflowerblue;'>S &mdash; </b><b>Minimum </b><b style='color:cornflowerblue';>Shoulder Taper:</b> <asp:Literal ID="ltlSTMin" runat="server" /> feet<br />
                            <b style='color:cornflowerblue;'>S &mdash; </b><b>Number of cones in </b><b style='color:cornflowerblue'>Shoulder Taper:</b> <asp:Literal ID="ltlSTCones" runat="server" /><br />
                            <b style='color:cornflowerblue;'>S &mdash; </b><b style='color:cornflowerblue'>Shoulder Taper</b> <b>cone spacing:</b> <asp:Literal ID="ltlSTConeSpacing" runat="server" /><br /><br />
                            <b> &mdash; TOTAL NUMBER OF CONES REQUIRED:</b> <asp:Literal ID="ltlConesReq" runat="server" /><br /><br />
                            <h5>Optional</h5>
                            <b style='color:deeppink;'>B &mdash; </b><b>Optional </b><b style='color:deeppink';>Buffer Zone </b><b>length:</b> <asp:Literal ID="ltlBZLength" runat="server" /> feet<br />
                            <b style='color:deeppink;'>B &mdash; </b><b>Optional </b><b style='color:deeppink';>Buffer Zone </b><b>cone spacing:</b> <asp:Literal ID="ltlBZConeSpacing" runat="server" /> feet<br />
                            <b style='color:deeppink;'>B &mdash; </b><b>Number of cones in the optional </b><b style='color:deeppink';>Buffer Zone</b><b>:</b> <asp:Literal ID="ltlBZCones" runat="server" /><br />
                            <b style='color:limegreen;'>D &mdash; </b><b>Optional </b><b style='color:limegreen';>Downstream Taper </b><b>length:</b> 100 feet<br />
                            <b style='color:limegreen;'>D &mdash; </b><b>Optional </b><b style='color:limegreen';>Downstream Taper </b><b>cone spacing:</b> 20 feet<br />
                            <b style='color:limegreen;'>D &mdash; </b><b>Number of cones in the optional </b><b style='color:limegreen';>Downstream Taper</b><b>:</b> 5<br />
                        </asp:Panel>

                        <asp:Panel ID="pnlIVbData" runat="server" CssClass="pnlCalcDataPrint" ClientIDMode="Static">
                            <h4>TCP Calculator Data</h4>
                            <b style='color:red;'>WS* &mdash; </b><b>Advanced</b> <b style='color:red;'> Warning Sign </b><b>Spacing per sign:</b> <asp:Literal ID="ltlWS1" runat="server" /> feet<br />
                            <b style='color:orange;'>WZ** &mdash; Work Zone </b><b>Length**:</b> <asp:Literal ID="ltlWZLength1" runat="server" /> feet<br />
                            <b style='color:orange;'>WZ** &mdash; </b><b>Number of cones in the</b> <b style='color:orange;'>Work Zone:</b> <asp:Literal ID="ltlWZCones1" runat="server" /><br />
                            <b style='color:orange;'>WZ** &mdash; Work Zone </b><b>cone spacing:</b> <asp:Literal ID="ltlWZConeSpacing1" runat="server" /> feet<br />
                            <b style='color:cornflowerblue;'>S &mdash; </b><b>Minimum </b><b style='color:cornflowerblue';>Shoulder Taper:</b> <asp:Literal ID="ltlSTMin1" runat="server" /> feet<br />
                            <b style='color:cornflowerblue;'>S &mdash; </b><b>Number of cones in </b><b style='color:cornflowerblue'>Shoulder Taper:</b> <asp:Literal ID="ltlSTCones1" runat="server" /><br />
                            <b style='color:cornflowerblue;'>S &mdash; </b><b style='color:cornflowerblue'>Shoulder Taper</b> <b>cone spacing:</b> <asp:Literal ID="ltlSTConeSpacing1" runat="server" /><br /><br />
                            <b> &mdash; TOTAL NUMBER OF CONES REQUIRED:</b> <asp:Literal ID="ltlConesReq1" runat="server" /><br /><br />
                            <h5>Optional</h5>
                            <b style='color:deeppink;'>B &mdash; </b><b>Optional </b><b style='color:deeppink';>Buffer Zone </b><b>length:</b> <asp:Literal ID="ltlBZLength1" runat="server" /> feet<br />
                            <b style='color:deeppink;'>B &mdash; </b><b>Optional </b><b style='color:deeppink';>Buffer Zone </b><b>cone spacing:</b> <asp:Literal ID="ltlBZConeSpacing1" runat="server" /> feet<br />
                            <b style='color:deeppink;'>B &mdash; </b><b>Number of cones in the optional </b><b style='color:deeppink';>Buffer Zone</b><b>:</b> <asp:Literal ID="ltlBZCones1" runat="server" /><br />
                            <b style='color:limegreen;'>D &mdash; </b><b>Optional </b><b style='color:limegreen';>Downstream Taper </b><b>length:</b> 100 feet<br />
                            <b style='color:limegreen;'>D &mdash; </b><b>Optional </b><b style='color:limegreen';>Downstream Taper </b><b>cone spacing:</b> 20 feet<br />
                            <b style='color:limegreen;'>D &mdash; </b><b>Number of cones in the optional </b><b style='color:limegreen';>Downstream Taper</b><b>:</b> 5<br />
                        </asp:Panel>

                        <asp:Panel ID="pnlLessData" runat="server" CssClass="pnlCalcDataPrint" ClientIDMode="Static">
                            <h4>TCP Calculator Data</h4>
                            <b style='color:red;'>WS* &mdash; </b><b>Optional Advanced</b> <b style='color:red;'>Warning Sign</b> <b>Spacing per sign:</b> <asp:Literal ID="ltlWS2" runat="server" /> feet<br />
                            <b style='color:orange;'>WZ** &mdash; Work Zone </b><b>Length**:</b> <asp:Literal ID="ltlWZLength2" runat="server" /> feet<br />
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Panel ID="pnlImage" runat="server">
                            <asp:Image ID="imgPlanImage" runat="server" CssClass="TCPPlanImagePrint" ClientIDMode="Static" />
                        </asp:Panel>
                    </td>
                </tr>
            </table>

        </div>
    </div>
    </form>
</body>
</html>
