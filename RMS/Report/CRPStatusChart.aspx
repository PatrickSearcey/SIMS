<%@ Page Title="" Language="C#" MasterPageFile="~/RMSSingleMenu.Master" AutoEventWireup="true" CodeBehind="CRPStatusChart.aspx.cs" Inherits="RMS.Report.CRPStatusChart" %>
<%@ Register Src="~/Control/RecordPageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../styles/crpstatus.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <telerik:RadAjaxManager ID="ram" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="rddlOffice">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rhcCRPStatus" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="ralp" runat="server" Skin="Bootstrap" />

    <uc:PageHeading id="ph1" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph2" runat="server">
        <br />
    <div class="mainContent">
        <div class="Filters">
            <table width="100%">
                <tr>
                    <td width="700">Choose an office to view progress by record-type, or select All Offices to view progress by office:</td>
                    <td><telerik:RadDropDownList ID="rddlOffice" runat="server" DataValueField="office_id" DataTextField="office_nm" Skin="Bootstrap" Width="350px"
                        DropDownHeight="200px" OnSelectedIndexChanged="UpdateDetails" AutoPostBack="true" /></td>
                    <td><asp:Literal ID="ltlError" runat="server" /></td>
                </tr>
                <tr>
                    <td><span class="filtersSubtext"><asp:Literal ID="ltl150DaysAgo" runat="server" /> &nbsp;&nbsp;&nbsp;<asp:Literal ID="ltl240DaysAgo" runat="server" /></span></td>
                    <td colspan="2"><span class="filtersSubtext"><asp:Literal ID="ltlCurrentData" runat="server" /></span></td>
                </tr>
            </table>
        </div>
        <telerik:RadPanelBar ID="rpbExplanation" runat="server" Width="100%" Skin="Bootstrap">
            <Items>
                <telerik:RadPanelItem Text="Explanation (click here to collapse/expand)" Expanded="false">
                    <ContentTemplate>
                        <div style="padding:10px;font-size:9pt;">
                            <p>The labels above each bar display how many records meet all criteria for the category per total number of records for that category.</p>
                        </div>
                    </ContentTemplate>
                </telerik:RadPanelItem>
            </Items>
        </telerik:RadPanelBar>
        <hr />
        <telerik:RadHtmlChart runat="server" ID="rhcCRPStatus" Width="1000" Height="500" Transitions="true" Skin="Bootstrap">
            <PlotArea>
                <Series>
                    <telerik:ColumnSeries Name="Category 1" Stacked="false" Gap="1.5" Spacing="0.4" DataFieldY="Cat1Percent">
                        <Appearance>
                            <FillStyle BackgroundColor="#c5d291" />
                        </Appearance>
                        <LabelsAppearance Position="OutsideEnd">
                            <ClientTemplate>
                                #=dataItem.RecordsThatMeetCat1Criteria# / #=dataItem.TotalCat1Records#
                            </ClientTemplate>
                        </LabelsAppearance>
                        <TooltipsAppearance BackgroundColor="#b84626" Color="White">
                            <ClientTemplate>
                                #=dataItem.Cat1Percent#%
                            </ClientTemplate>
                        </TooltipsAppearance>
                    </telerik:ColumnSeries>
                    <telerik:ColumnSeries Name="Category 2" Stacked="false" Gap="1.5" Spacing="0.4" DataFieldY="Cat2Percent">
                        <Appearance>
                            <FillStyle BackgroundColor="#5cb8e3" />
                        </Appearance>
                        <LabelsAppearance Position="OutsideEnd">
                            <ClientTemplate>
                                #=dataItem.RecordsThatMeetCat2Criteria# / #=dataItem.TotalCat2Records#
                            </ClientTemplate>
                        </LabelsAppearance>
                        <TooltipsAppearance BackgroundColor="#b84626" Color="White">
                            <ClientTemplate>
                                #=dataItem.Cat2Percent#%
                            </ClientTemplate>
                        </TooltipsAppearance>
                    </telerik:ColumnSeries>
                </Series>
                <Appearance>
                    <FillStyle BackgroundColor="Transparent"></FillStyle>
                </Appearance>
                <XAxis AxisCrossingValue="0" Color="black" MajorTickType="Outside" MinorTickType="Outside" DataLabelsField="RecordType"
                    Reversed="false">
                    <LabelsAppearance DataFormatString="{0}" RotationAngle="20" Skip="0" Step="1"></LabelsAppearance>
                    <TitleAppearance Visible="False"></TitleAppearance>
                </XAxis>
                <YAxis AxisCrossingValue="0" Color="black" MajorTickSize="1" MajorTickType="Outside" MaxValue="100" MinValue="0"
                    MinorTickType="None" Reversed="false">
                    <PlotBands>
                        <telerik:PlotBand From="79" To="80" Color="#009933" />
                    </PlotBands>
                    <LabelsAppearance DataFormatString="{0}%" RotationAngle="0" Skip="0" Step="1"></LabelsAppearance>
                    <TitleAppearance Position="Center" RotationAngle="0" Text="% of records done"></TitleAppearance>
                </YAxis>
            </PlotArea>
            <Appearance>
                <FillStyle BackgroundColor="Transparent"></FillStyle>
            </Appearance>
            <ChartTitle Text="Record Progress">
                <Appearance Visible="false"></Appearance>
            </ChartTitle>
            <Legend>
                <Appearance BackgroundColor="Transparent" Position="Top" OffsetY="-10"></Appearance>
            </Legend>
        </telerik:RadHtmlChart>
    </div>
</asp:Content>
