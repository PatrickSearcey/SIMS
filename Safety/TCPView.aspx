<%@ Page Title="" Language="C#" MasterPageFile="~/SafetySingleMenu.Master" AutoEventWireup="true" CodeBehind="TCPView.aspx.cs" Inherits="Safety.TCPView" %>
<%@ Register Src="~/Control/SitePageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="styles/reports.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <uc:PageHeading id="ph1" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph2" runat="server">

    <!-- ADD THE TRAFFIC CONTROL PLAN IN THIS CONTENT PLACE HOLDER -->
       <link href="Content/Slider.css" rel="stylesheet" />
    <script src="Scripts/jquery-1.9.1.min.js"></script>
    <link href="Content/bootstrap-theme.min.css" rel="stylesheet" />
    <link href="Content/bootstrap.css" rel="stylesheet" />
    <style>
        fieldset {
            border: 0;
        }

        label {
            display: block;
            margin: 5px 0 0 5px;
        }

        .overflow {
            height: 200px;
        }
    </style>
    <script src="Scripts/bootstrap.min.js"></script>
    <script src="Scripts/jquery-ui-1.12.1.custom/jquery-ui.min.js"></script>
    <telerik:RadAjaxPanel ID="Rap1" runat="server">
        <asp:Literal ID="ltlTCPtitle" runat="server" />
        <asp:Panel runat="server">
            <img src="images/TCPPlanIa.png" style="float: right; width: 60%;" />
        </asp:Panel>

        <asp:Panel Style="border: solid" Width="40%" Height="40%" runat="server">
            <label for="name"><b>Site Name: </b><asp:Literal ID="ltlName" runat="server" /></label>

            <label for="number"><b>Station Number: </b><asp:Literal ID="ltlNumber" runat="server" /></label>           

            <label for="highway"><b>Highway/Road: </b><asp:Literal ID="ltlHighway" runat="server" /></label>    

            <label for="freeway"><b>Freeway or Expressway: </b><asp:Literal ID="ltlFreeway" runat="server" /></label>            

            <label for="width"><b>Bridge Width: </b><asp:Literal ID="ltlWidth" runat="server" /></label>          

            <label for="length"><b>Bridge Length: </b><asp:Literal ID="ltlWorkZone" runat="server" /></label>           

            <label for="lane"><b>Lane Width: </b><asp:Literal ID="ltlLane" runat="server" /></label>          

            <label for="shoulder"><b>Shoulder Width: </b><asp:Literal ID="ltlShoulder" runat="server" /></label>         

            <label for="speed"><b>Speed Limit: </b><asp:Literal ID="ltlSpeed" runat="server" /></label>            

            <label for="traffic"><b>Traffic Volume: </b><asp:Literal ID="ltlTraffic" runat="server" /></label>            

            <label for="cell"><b>Cell Service Available: </b><asp:Literal ID="ltlCell" runat="server" /></label>
            
            <label for="updated"><b>Plan Updated: </b><asp:Literal ID="ltlUpdated" runat="server" /></label>           

            <label for="updated"><b>Plan Reviewed: </b><asp:Literal ID="ltlReviewed" runat="server" /></label>           

            <label for="approved"><b>Plan Approved: </b><asp:Literal ID="ltlApproved" runat="server" /></label>           

            <label for="notes"><b>Site Specific Notes:</b></label>
            <asp:Literal ID="ltlNotes" runat="server" />

            <label for="remarks"><b>Remarks:</b></label>
            <asp:Literal ID="ltlRemarks" runat="server" />
        </asp:Panel>

        <asp:Panel runat="server">
            <label for="data"><b>TCP Calculator Data</b></label>
            <asp:Literal ID="ltlData" runat="server" />
            <b style='color:red;'>WS* &mdash; </b><b>Advanced</b> <b style='color:red;'> Warning Sign </b><b>Spacing per sign:</b>[value] <br />
            <b style='color:grey;'>F &mdash; Flagger</b> <b>Distance from the shoulder taper:</b> [value]<br />
            <b style='color:orange;'>WZ** &mdash; Work Zone </b><b>Length**:</b>[value]<br />
            <b style='color:orange;'>WZ** &mdash; </b><b>Number of cones in the</b> <b style='color:orange;'>Work Zone:</b>[value]<br />
            <b style='color:orange;'>WZ** &mdash; Work Zone </b><b>cone spacing:</b>[value]<br />
            <b style='color:mediumpurple;'>T &mdash; </b><b>Minimum </b><b style='color:mediumpurple';>Transition Taper </b><b>length:</b>[value]<br />
            <b style='color:mediumpurple;'>T &mdash; </b><b>Number of cones in the </b><b style='color:mediumpurple';>Transition Taper</b><b>:</b>[value]<br />
            <b style='color:mediumpurple;'>T &mdash; </b><b>Maximum spacing for cones in the </b><b style='color:mediumpurple';>Transition Taper</b><b>:</b>[value]<br />
            <b style='color:cornflowerblue;'>S &mdash; </b><b>Minimum </b><b style='color:cornflowerblue';>Shoulder Taper</b><b>:</b>[value]<br />
            <b style='color:cornflowerblue;'>S &mdash; </b><b>Number of cones in </b><b style='color:cornflowerblue';>Shoulder Taper</b><b>:</b>[value]<br />
            <b> &mdash; TOTAL NUMBER OF CONES REQUIRED:</b>[value]<br />
            <b style='color:deeppink;'>B &mdash; </b><b>Optional </b><b style='color:deeppink';>Buffer Zone </b><b>length:</b>[value]<br />
            <b style='color:deeppink;'>B &mdash; </b><b>Optional </b><b style='color:deeppink';>Buffer Zone </b><b>cone spacing:</b>[value]<br />
            <b style='color:deeppink;'>B &mdash; </b><b>Number of cones in the optional </b><b style='color:deeppink';>Buffer Zone</b><b>:</b>[value]<br />
            <b style='color:limegreen;'>D &mdash; </b><b>Optional </b><b style='color:limegreen';>Downstream Taper </b><b>length:</b>[value]<br />
            <b style='color:limegreen;'>D &mdash; </b><b>Optional </b><b style='color:limegreen';>Downstream Taper </b><b>cone spacing:</b>[value]<br />
            <b style='color:limegreen;'>D &mdash; </b><b>Number of cones in the optional </b><b style='color:limegreen';>Downstream Taper</b><b>:</b>[value]<br />
        </asp:Panel>
         
        <asp:Panel Style="border: solid" Width="40%" runat="server">
            <label for="instructions"><b>Instructions</b></label>
            <asp:Literal ID="ltlInstructions" runat="server" />
        </asp:Panel>
    </telerik:RadAjaxPanel>
</asp:Content>
