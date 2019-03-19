<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="FieldTripMap.aspx.vb" Inherits="SIMS.FieldTripMap" %>
<%@ Register Assembly="GMaps" Namespace="Subgurim.Controles" TagPrefix="cc1" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxtoolkit" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Site Information Management System - Field Trip Maps</title>
    <link rel="stylesheet" type="text/css" href="css/styles.css" />
    <link rel="stylesheet" type="text/css" href="css/common.css" />
    <link rel="stylesheet" type="text/css" href="css/custom.css" />
    <style type="text/css">
    v\:* { behavior:url(#default#VML); }
    /*------------FOR UPDATEPROGRESS -----------------*/
    .overlay {
        position: fixed;
        z-index: 99;
        top: 0px;
        left: 0px;
        background-color: #FFFFFF;
        width: 100%;
        height: 100%;
        filter: Alpha(Opacity=70);
        opacity: 0.70;
        -moz-opacity: 0.70;
    }
    * html .overlay 
    {
        position: absolute;
        height: expression(document.body.scrollHeight > document.body.offsetHeight ? document.body.scrollHeight : document.body.offsetHeight + 'px');
        width: expression(document.body.scrollWidth > document.body.offsetWidth ? document.body.scrollWidth : document.body.offsetWidth + 'px');
    }
    .loader 
    {
        z-index: 100;
        position: fixed;
        width: 120px;
        margin-left: -60px;
        top: 50%;
        left: 50%;
    }
    * html .loader 
    {
        position: absolute;
        margin-top: expression((document.body.scrollHeight / 4) + (0 - parseInt(this.offsetParent.clientHeight / 2) + (document.documentElement && document.documentElement.scrollTop || document.body.scrollTop)) + 'px');
    }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <ajaxtoolkit:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </ajaxtoolkit:ToolkitScriptManager>
    <telerik:RadFormDecorator ID="FormDecorator1" runat="server" DecoratedControls="all"></telerik:RadFormDecorator>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="800">
        <ProgressTemplate>
            <asp:Panel ID="Panel1" CssClass="overlay" runat="server">
                <asp:Panel ID="Panel2" CssClass="loader" runat="server">
                    <span class="SITitleFontSmall">Processing...</span>                        
                </asp:Panel>
            </asp:Panel>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <div>
    <p class="SITitleFont" style="padding-top:10px;text-align:center;">Field Trip Maps</p>
    <asp:UpdatePanel ID="upMain" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel id="pnlMap_Canvas" runat="server" CssClass="Map_Canvas">
                <asp:Panel ID="pnlSideFilterControls" runat="server" CssClass="SideFilterControls">
                    <div style="padding:5px;">
                        <asp:Panel ID="pnlOffice" runat="server">
                            <p class="SITitleFontSmall" style="margin-top:0px;">Change the office:</p>
                            <asp:Panel ID="pnlOfficeIP" runat="server" Width="255" BackColor="#FFFFFF">
                                <p style="padding-left:3px;line-height:18px;">
                                <asp:DropDownList ID="ddlOffice" runat="server" OnSelectedIndexChanged="ddlOffice_SelectedIndexChanged" 
                                    AutoPostBack="True" DataTextField="office_nm" DataValueField="office_id"
                                    CssClass="SIBodyFontSmall" />
                                </p>
                            </asp:Panel>
                        </asp:Panel>
                    
                        <asp:Panel ID="pnlNotice" runat="server" Visible="false">
                            <p class="SITitleFontSmall" style="margin-top:0px;">Notice:</p>
                            <asp:Panel ID="pnlNoticeIP" runat="server" Width="255" BackColor="#FFFFFF">
                                <p style="padding-left:3px;line-height:18px;">
                                <asp:Label ID="lblNotice" runat="server" Font-Bold="true" ForeColor="Red" />
                                </p>
                            </asp:Panel>
                            <br />
                        </asp:Panel>
                        
                        <asp:Panel ID="pnlFilters" runat="server">
                            <br />
                            <p class="SITitleFontSmall" style="margin-top:0px;">Select field trips to display:</p>
                            <asp:Panel ID="pnlDisplayOptions" Width="255" runat="server" BackColor="#FFFFFF">
                                <p style="padding-left:3px;line-height:18px;">
                                <asp:CheckBoxList ID="cblFieldTrips" runat="server" DataValueField="trip_id" 
                                    AutoPostBack="true" DataTextField="trip_nm_full" 
                                    OnSelectedIndexChanged="cblFieldTrips_SelectedIndexChanged">
                                </asp:CheckBoxList>
                                </p>
                            </asp:Panel>
                        </asp:Panel>
                        
                        <asp:Panel ID="pnlResults" runat="server" Visible="false">
                            <br />
                            <p class="SITitleFontSmall" style="margin-top:0px;">Results:</p>
                            <asp:Panel ID="pnlResultsIP" runat="server" Width="255" BackColor="#FFFFFF">
                                <p style="padding-left:3px;line-height:18px;">
                                <asp:Label ID="lblResultsCount" runat="server" cssclass="SIBodyFontSmall" Font-Bold="true" />
                                </p>
                            </asp:Panel>
                        </asp:Panel>
                        
                    </div>
                </asp:Panel>
                <asp:Panel id="pnlMap" runat="server" CssClass="Map">
                    <cc1:gmap id="GMap" runat="server" Width="100%" Height="100%" enableServerEvents="False"></cc1:gmap>
                </asp:Panel>
            </asp:Panel>
            <ajaxToolkit:RoundedCornersExtender ID="rce1" runat="server" 
                TargetControlID="pnlOfficeIP" 
                BorderColor="#390079" Enabled="True" Radius="10" />
            <ajaxToolkit:RoundedCornersExtender ID="rce2" runat="server" 
                TargetControlID="pnlDisplayOptions" 
                BorderColor="#390079" Enabled="True" Radius="10" />
            <ajaxToolkit:RoundedCornersExtender ID="rce3" runat="server" 
                TargetControlID="pnlNoticeIP" 
                BorderColor="#390079" Enabled="True" Radius="10" />
            <ajaxToolkit:RoundedCornersExtender ID="rce4" runat="server" 
                TargetControlID="pnlResultsIP" 
                BorderColor="#390079" Enabled="True" Radius="10" />
        </ContentTemplate>
    </asp:UpdatePanel>
    </div>
    <p>References to non-U.S. Department of the Interior (DOI) products do not constitute an endorsement 
    by the DOI. By viewing the Google Maps API on this web site the user agrees to these 
    <a href="http://code.google.com/apis/maps/terms.html">TERMS</a> of Service set forth by Google. 
    </p>
    <p class="footerBar" style="clear: both;">&nbsp;</p>
    <p class="footerText"><a href="http://internal.usgs.gov">U.S. Geological Survey Intranet</a><br />
	    URL: <a href="https://sims.water.usgs.gov/">https://sims.water.usgs.gov/</a><br />
	    Page Contact Information: <a href="mailto:GS-W_Help_SIMS@usgs.gov">GS-W_Help_SIMS@usgs.gov</a><br />
	    Page Last Updated: 08/30/2017</p>
    </form>
</body>
</html>
