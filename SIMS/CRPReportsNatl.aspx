<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/SIMSSite.Master" CodeBehind="CRPReportsNatl.aspx.vb" Inherits="SIMS.CRPReportsNatl" %>
<%@ MasterType virtualPath="~/SIMSSite.master" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxtoolkit" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Charting" tagprefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
<telerik:RadFormDecorator ID="FormDecorator1" runat="server" DecoratedControls="all" Skin="Sunset"></telerik:RadFormDecorator>
<p class="SITitleFont">National Continuous Records Processing Charts</p>
<h4><asp:Literal ID="ltlPageSubTitle" runat="server" /></h4>
<telerik:RadAjaxPanel ID="rapMain" runat="server">
    <table width="1000">
        <tr>
            <td valign="top">
                <fieldset style="height:50px;">
                    <legend>Explanation</legend>
                    <div style="padding:5px;">
                        <label>The labels above each bar display how many records meet all criteria for the category
                        per total number of records for that category.</label>
                    </div>
                </fieldset>
            </td>
        </tr>
        <tr>
            <td>
            <br /><br />
                <h4><asp:Literal ID="ltlRC1Title" runat="server" /></h4>
            </td>
        </tr>
        <tr>
            <td>
                <telerik:RadChart ID="rcCat1" runat="server" width="1000px"
                    PlotArea-YAxis-Appearance-MajorGridLines-Color="#993300" 
                    PlotArea-YAxis-Appearance-MajorGridLines-Visible="true" 
                    PlotArea-YAxis-AxisLabel-Visible="true" AutoLayout="true"
                    PlotArea-YAxis-AxisLabel-TextBlock-Text="% of records done" 
                    AutoTextWrap="true" Skin="Sunset">
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
            </td>
        </tr>
        <tr>
            <td align="right">
                <asp:Label ID="lblDate1" runat="server" Font-Italic="true" ForeColor="SaddleBrown" />
            </td>
        </tr>
        <tr>
            <td>
                <h4><asp:Literal ID="ltlRC2Title" runat="server" /></h4>
            </td>
        </tr>
        <tr>
            <td> 
                <telerik:RadChart ID="rcCat2" runat="server" width="1000px"
                    PlotArea-YAxis-Appearance-MajorGridLines-Color="#993300" 
                    PlotArea-YAxis-Appearance-MajorGridLines-Visible="true" 
                    PlotArea-YAxis-AxisLabel-Visible="true" AutoLayout="true"
                    PlotArea-YAxis-AxisLabel-TextBlock-Text="% of records done" 
                    AutoTextWrap="true" Skin="Sunset">
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
            </td>
        </tr>
        <tr>
            <td align="right">
                <asp:Label ID="lblDate2" runat="server" Font-Italic="true" ForeColor="SaddleBrown" />
            </td>
        </tr>
        <tr>
            <td>
                <h4><asp:Literal ID="ltlRC3Title" runat="server" /></h4>
            </td>
        </tr>
        <tr>
            <td>
                <telerik:RadChart ID="rcCat3" runat="server" width="1000px"
                    PlotArea-YAxis-Appearance-MajorGridLines-Color="#993300" 
                    PlotArea-YAxis-Appearance-MajorGridLines-Visible="true" 
                    PlotArea-YAxis-AxisLabel-Visible="true" AutoLayout="true"
                    PlotArea-YAxis-AxisLabel-TextBlock-Text="% of records done" 
                    AutoTextWrap="true" Skin="Sunset">
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
            </td>
        </tr>
        <tr>
            <td align="right">
                <asp:Label ID="lblDate3" runat="server" Font-Italic="true" ForeColor="SaddleBrown" />
            </td>
        </tr>
    </table>
</telerik:RadAjaxPanel>
</asp:Content>
