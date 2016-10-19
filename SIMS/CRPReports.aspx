<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/RMS.Master" CodeBehind="CRPReports.aspx.vb" Inherits="SIMS.CRPReports" %>
<%@ MasterType  virtualPath="~/RMS.master" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxtoolkit" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Charting" tagprefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" Runat="Server">
<asp:Panel ID="pnlHasAccess" runat="server">
    <telerik:RadAjaxPanel ID="rapMain" runat="server">
        <table width="1000">
            <tr>
                <td valign="top" width="250px">
                    Choose a WSC: <br />
                    <asp:DropDownList ID="ddlWSC" runat="server" OnSelectedIndexChanged="ddlWSC_SelectedIndexChanged"
                        DataTextField="wsc_nm" DataValueField="wsc_id" AutoPostBack="true" />
                    <asp:Panel ID="pnlOffice" runat="server" Visible="false">
                        Choose an Office: <br />
                        <asp:DropDownList ID="ddlOffice" runat="server" AutoPostBack="true" onSelectedIndexChanged="ddlOffice_SelectedIndexChanged"
                            DataTextField="office_nm" DataValueField="office_id" />
                    </asp:Panel><br />
                </td>
                <td valign="top" width="250px">
                    <asp:Panel ID="pnlGraph" runat="server">
                        <fieldset style="width:250px;text-align:center;height:95px;">
                            <legend>Report View</legend>
                            <div style="padding:5px;">
                                <asp:HyperLink ID="hlGraph" runat="server" ImageUrl="images/reportSS.png" />
                            </div>
                        </fieldset>
                    </asp:Panel>
                </td>
                <td valign="top" width="250px">
                    <fieldset style="height:95px;">
                        <legend>Navigation Guide</legend>
                        <div style="padding:5px;">
                            <asp:Image id="imgProgress" runat="server" /><br />
                            <label>Click on the bars in the chart to drill down. </label>
                            <!--Select from the drop-down below the data
                             to display in the final chart.<br />-->
                            <asp:DropDownList ID="ddlFinalChartData" runat="server" visible="false">
                                <asp:ListItem Value="records">progress by record-type</asp:ListItem>
                                <asp:ListItem Value="userid">progress by employee ID</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </fieldset>
                </td>
                <td valign="top">
                    <fieldset style="height:95px;">
                        <legend>Explanation</legend>
                        <div style="padding:5px;">
                            <label style="font-size:0.77em;">The labels above each bar display how many records meet all criteria for the category
                            per total number of records for that category.</label>
                        </div>
                    </fieldset>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <telerik:RadChart ID="rcCat1" runat="server" width="1000px"
                        PlotArea-YAxis-Appearance-MajorGridLines-Color="#993300" 
                        PlotArea-YAxis-Appearance-MajorGridLines-Visible="true" 
                        PlotArea-YAxis-AxisLabel-Visible="true" AutoLayout="true"
                        PlotArea-YAxis-AxisLabel-TextBlock-Text="% of records done" 
                        AutoTextWrap="true" Skin="Sunset"
                        OnClick="rcCat1_Click">
                        <Legend TextBlock-Visible="true" TextBlock-Text="Explanation" Appearance-ItemTextAppearance-AutoTextWrap="False">
                        </Legend>
                        <ChartTitle TextBlock-Appearance-TextProperties-Font="18pt" />
                        <PlotArea>
                            <MarkedZones>
                                <telerik:ChartMarkedZone ValueStartY="79" ValueEndY="80">
                                    <Appearance FillStyle-MainColor="green">
                                    </Appearance>
                                </telerik:ChartMarkedZone>
                            </MarkedZones>
                            <XAxis>
                                <Appearance>
                                    <TextAppearance TextProperties-Font="Arial, 8.25pt, style=Bold" />
                                </Appearance>
                            </XAxis>
                            <YAxis AutoScale="false" MaxValue="100" MinValue="0" LabelStep="10">
                                <Appearance>
                                    <TextAppearance TextProperties-Font="Arial, 8.25pt, style=Bold" />
                                </Appearance>
                            </YAxis>
                        </PlotArea>
                    </telerik:RadChart>
                    <asp:HiddenField ID="hfSeriesName" runat="server" />
                    <asp:HiddenField ID="hfOfficeID" runat="server" />
                    <asp:HiddenField ID="hfWSCID" runat="server" />
                    <telerik:RadToolTipManager ID="rttmCat1" runat="server" Skin="Sunset"
                        Width="200px" Animation="Slide" Position="TopCenter" ToolTipZoneID="rcCat1" AutoTooltipify="true">
                    </telerik:RadToolTipManager>    
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Button ID="btnWSCReset" runat="server" Text="reset to WSC level" OnCommand="Reset_Command" CommandArgument="ResettoWSC" />
                    <asp:Button ID="btnOfficeReset" runat="server" Text="reset to office level" OnCommand="Reset_Command" CommandArgument="ResettoOffice" />
                </td>
                <td colspan="2" align="right">
                    <asp:Label runat="server" ID="lblDateCount" Font-italic="true" ForeColor="SaddleBrown" /><br />
                    <asp:Label ID="lblDate" runat="server" Font-Italic="true" ForeColor="SaddleBrown" />
                </td>
            </tr>
        </table>
    </telerik:RadAjaxPanel>
</asp:Panel>
</asp:Content>
