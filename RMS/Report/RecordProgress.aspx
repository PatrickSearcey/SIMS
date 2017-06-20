<%@ Page Title="" Language="C#" MasterPageFile="~/RMSSingleMenu.Master" AutoEventWireup="true" CodeBehind="RecordProgress.aspx.cs" Inherits="RMS.Report.RecordProgress" %>
<%@ Register Src="~/Control/RecordPageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../styles/recordprogress.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <telerik:RadAjaxManager ID="ram" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="rbSubmit">
                <UpdatedControls>
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
            <p>
                Select the desired office and time-frame to customize the Record Progress report.
            </p>
            <table>
                <tr>
                    <td><telerik:RadDropDownList ID="rddlOffice" runat="server" DataValueField="office_id" DataTextField="office_nm" Skin="Bootstrap" Width="350px" DropDownHeight="200px" /></td>
                    <td>Progress Through: <telerik:RadDatePicker ID="rdpEndDt" runat="server" Skin="Bootstrap" DateInput-EmptyMessage="progress through" Width="150px" /></td>
                    <td><telerik:RadButton ID="rbSubmit" runat="server" OnCommand="UpdateDetails" CommandArgument="Update" Text="Submit" AutoPostBack="true" Skin="Bootstrap" /></td>
                    <td><asp:Literal ID="ltlError" runat="server" /></td>
                </tr>
            </table>
        </div>
        <div class="Charts">
            <div class="AllRecords">
                <h3>All Records</h3>
                <p style="padding-top:0;margin-top:0">Total Records = <asp:Literal ID="ltlAllRecordsTR" runat="server" /></p>
                <telerik:RadHtmlChart runat="server" ID="rhcAllRecords" Width="400" Height="250" Transitions="true" Skin="Bootstrap">
                    <PlotArea>
                        <Series>
                            <telerik:BarSeries Name="Analyzed" Stacked="false" Gap="1.5" Spacing="0.4" DataFieldY="AnalyzedPercent">
                                <Appearance>
                                    <FillStyle BackgroundColor="#c5d291"></FillStyle>
                                </Appearance>
                                <LabelsAppearance Visible="false"></LabelsAppearance>
                                <TooltipsAppearance BackgroundColor="#b84626" Color="White">
                                    <ClientTemplate>
                                        #=dataItem.AnalyzedPercentString# | Actual Number: #=dataItem.Analyzed#
                                    </ClientTemplate>
                                </TooltipsAppearance>
                            </telerik:BarSeries>
                            <telerik:BarSeries Name="Approved" Stacked="false" Gap="1.5" Spacing="0.4" DataFieldY="ApprovedPercent">
                                <Appearance>
                                    <FillStyle BackgroundColor="#5cb8e3"></FillStyle>
                                </Appearance>
                                <LabelsAppearance Visible="False"></LabelsAppearance>
                                <TooltipsAppearance BackgroundColor="#b84626" Color="White">
                                    <ClientTemplate>
                                        #=dataItem.ApprovedPercentString# | Actual Number: #=dataItem.Approved#
                                    </ClientTemplate>
                                </TooltipsAppearance>
                            </telerik:BarSeries>
                        </Series>
                        <Appearance>
                            <FillStyle BackgroundColor="Transparent"></FillStyle>
                        </Appearance>
                        <XAxis AxisCrossingValue="0" Color="black" MajorTickType="Outside" MinorTickType="Outside"
                            Reversed="false">
                            <LabelsAppearance DataFormatString="{0}" RotationAngle="0" Skip="0" Step="1"></LabelsAppearance>
                            <TitleAppearance Position="Center" RotationAngle="0" Text="Status"></TitleAppearance>
                        </XAxis>
                        <YAxis AxisCrossingValue="0" Color="black" MajorTickSize="1" MajorTickType="Outside" MaxValue="100" MinValue="0"
                            MinorTickType="None" Reversed="false">
                            <LabelsAppearance DataFormatString="{0}%" RotationAngle="0" Skip="0" Step="1"></LabelsAppearance>
                            <TitleAppearance Position="Center" RotationAngle="0" Text="Progress Percentage"></TitleAppearance>
                        </YAxis>
                    </PlotArea>
                    <Appearance>
                        <FillStyle BackgroundColor="Transparent"></FillStyle>
                    </Appearance>
                    <ChartTitle Text="All Records">
                        <Appearance Visible="false"></Appearance>
                    </ChartTitle>
                    <Legend>
                        <Appearance BackgroundColor="Transparent" Position="Bottom"></Appearance>
                    </Legend>
                </telerik:RadHtmlChart>
            </div>
            <div class="TSRecords">
                <h3>Time-Series Records</h3>
                <p style="padding-top:0;margin-top:0">Total Records = <asp:Literal ID="ltlTSRecordsTR" runat="server" /></p>
                <telerik:RadHtmlChart runat="server" ID="rhcTSRecords" Width="400" Height="250" Transitions="true" Skin="Bootstrap">
                    <PlotArea>
                        <Series>
                            <telerik:BarSeries Name="Analyzed" Stacked="false" Gap="1.5" Spacing="0.4" DataFieldY="AnalyzedPercent">
                                <Appearance>
                                    <FillStyle BackgroundColor="#c5d291"></FillStyle>
                                </Appearance>
                                <LabelsAppearance Visible="false"></LabelsAppearance>
                                <TooltipsAppearance BackgroundColor="#b84626" Color="White">
                                    <ClientTemplate>
                                        #=dataItem.AnalyzedPercentString# | Actual Number: #=dataItem.Analyzed#
                                    </ClientTemplate>
                                </TooltipsAppearance>
                            </telerik:BarSeries>
                            <telerik:BarSeries Name="Approved" Stacked="false" Gap="1.5" Spacing="0.4" DataFieldY="ApprovedPercent">
                                <Appearance>
                                    <FillStyle BackgroundColor="#5cb8e3"></FillStyle>
                                </Appearance>
                                <LabelsAppearance Visible="False"></LabelsAppearance>
                                <TooltipsAppearance BackgroundColor="#b84626" Color="White">
                                    <ClientTemplate>
                                        #=dataItem.ApprovedPercentString# | Actual Number: #=dataItem.Approved#
                                    </ClientTemplate>
                                </TooltipsAppearance>
                            </telerik:BarSeries>
                        </Series>
                        <Appearance>
                            <FillStyle BackgroundColor="Transparent"></FillStyle>
                        </Appearance>
                        <XAxis AxisCrossingValue="0" Color="black" MajorTickType="Outside" MinorTickType="Outside"
                            Reversed="false">
                            <LabelsAppearance DataFormatString="{0}" RotationAngle="0" Skip="0" Step="1"></LabelsAppearance>
                            <TitleAppearance Position="Center" RotationAngle="0" Text="Status"></TitleAppearance>
                        </XAxis>
                        <YAxis AxisCrossingValue="0" Color="black" MajorTickSize="1" MajorTickType="Outside" MaxValue="100" MinValue="0"
                            MinorTickType="None" Reversed="false">
                            <LabelsAppearance DataFormatString="{0}%" RotationAngle="0" Skip="0" Step="1"></LabelsAppearance>
                            <TitleAppearance Position="Center" RotationAngle="0" Text="Progress Percentage"></TitleAppearance>
                        </YAxis>
                    </PlotArea>
                    <Appearance>
                        <FillStyle BackgroundColor="Transparent"></FillStyle>
                    </Appearance>
                    <ChartTitle Text="Time-Series Records">
                        <Appearance Visible="false"></Appearance>
                    </ChartTitle>
                    <Legend>
                        <Appearance BackgroundColor="Transparent" Position="Bottom"></Appearance>
                    </Legend>
                </telerik:RadHtmlChart>
            </div>
        </div>
    </div>
</asp:Content>
