<%@ Page Title="" Language="C#" MasterPageFile="~/RMSSingleMenu.Master" AutoEventWireup="true" CodeBehind="RecordProgressWSC.aspx.cs" Inherits="RMS.Report.RecordProgressWSC" %>
<%@ Register Src="~/Control/RecordPageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../styles/recordprogress.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <telerik:RadAjaxManager ID="ram" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="rbSubmit">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rhcAllRecords" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="rhcTSRecords" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="rhcNTSRecords" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="ltlAllRecordsTR" />
                    <telerik:AjaxUpdatedControl ControlID="ltlTSRecordsTR" />
                    <telerik:AjaxUpdatedControl ControlID="ltlNTSRecordsTR" />
                    <telerik:AjaxUpdatedControl ControlID="rgOffice" LoadingPanelID="ralp" />
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
                Select the time-frame to customize the Record Progress report.
            </p>
            <table>
                <tr>
                    <td>Progress Through: <telerik:RadDatePicker ID="rdpEndDt" runat="server" Skin="Bootstrap" DateInput-EmptyMessage="progress through" Width="150px" /></td>
                    <td><telerik:RadButton ID="rbSubmit" runat="server" OnCommand="UpdateDetails" CommandArgument="Update" Text="Submit" AutoPostBack="true" Skin="Bootstrap" /></td>
                    <td><asp:Literal ID="ltlError" runat="server" /></td>
                </tr>
            </table>
        </div>
        <table class="Charts">
            <tr>
                <td>
                    <div>
                        <h3>All Records</h3>
                        <p style="padding-top:0;margin-top:0"><asp:Literal ID="ltlAllRecordsTR" runat="server" /></p>
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
                </td>
                <td>
                    <div>
                        <h3>Time-Series Records</h3>
                        <p style="padding-top:0;margin-top:0"><asp:Literal ID="ltlTSRecordsTR" runat="server" /></p>
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
                </td>
                <td>
                    <div>
                        <h3>Non-Time-Series Records</h3>
                        <p style="padding-top:0;margin-top:0"><asp:Literal ID="ltlNTSRecordsTR" runat="server" /></p>
                        <telerik:RadHtmlChart runat="server" ID="rhcNTSRecords" Width="400" Height="250" Transitions="true" Skin="Bootstrap">
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
                            <ChartTitle Text="Non-Time-Series Records">
                                <Appearance Visible="false"></Appearance>
                            </ChartTitle>
                            <Legend>
                                <Appearance BackgroundColor="Transparent" Position="Bottom"></Appearance>
                            </Legend>
                        </telerik:RadHtmlChart>
                    </div>
                </td>
            </tr>
        </table>
        <hr />
        <h2>Breakdown By Office</h2>
        <telerik:RadGrid RenderMode="Lightweight" ID="rgOffice" runat="server" GridLines="None" Skin="Bootstrap" OnNeedDataSource="rgOffice_NeedDataSource"
            OnItemDataBound="rgOffice_ItemDataBound" AllowPaging="False" PageSize="10" AllowSorting="True">
            <MasterTableView AutoGenerateColumns="False" DataKeyNames="office_id">
                <Columns>
                    <telerik:GridTemplateColumn DataField="office_id" HeaderText="Office" SortExpression="office_nm" UniqueName="office_nm">
                        <ItemTemplate>
                            <a href='RecordProgress.aspx?office_id=<%# Eval("office_id") %>'><%# Eval("office_nm") %></a>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn DataField="office_id" UniqueName="AllRecordsProgressChartColumn" HeaderText="Progress For All Records">
                        <ItemTemplate>
                            <div style="width: 400px; height: 170px;">
                                <telerik:RadHtmlChart ID="rhcAllProgress" runat="server" Width="400" Height="170" Skin="Bootstrap">
                                    <Legend>
                                        <Appearance BackgroundColor="Transparent" Position="Bottom"></Appearance>
                                    </Legend>
                                    <ChartTitle>
                                        <Appearance Visible="false"></Appearance>
                                    </ChartTitle>
                                    <PlotArea>
                                        <Series>
                                            <telerik:BarSeries Name="Analyzed" Stacked="false" Gap="1.5" Spacing="0.4" DataFieldY="PercentAllAnalyzed">
                                                <Appearance>
                                                    <FillStyle BackgroundColor="#c5d291"></FillStyle>
                                                </Appearance>
                                                <LabelsAppearance Visible="false"></LabelsAppearance>
                                                <TooltipsAppearance BackgroundColor="#b84626" Color="White">
                                                    <ClientTemplate>
                                                        #=dataItem.PercentAllAnalyzedString# | #=dataItem.AllAnalyzed# records have been analyzed out of #=dataItem.AllRecords#
                                                    </ClientTemplate>
                                                </TooltipsAppearance>
                                            </telerik:BarSeries>
                                            <telerik:BarSeries Name="Approved" Stacked="false" Gap="1.5" Spacing="0.4" DataFieldY="PercentAllApproved">
                                                <Appearance>
                                                    <FillStyle BackgroundColor="#5cb8e3"></FillStyle>
                                                </Appearance>
                                                <LabelsAppearance Visible="False"></LabelsAppearance>
                                                <TooltipsAppearance BackgroundColor="#b84626" Color="White">
                                                    <ClientTemplate>
                                                        #=dataItem.PercentAllApprovedString# | #=dataItem.AllApproved# records have been approved out of #=dataItem.AllRecords#
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
                                </telerik:RadHtmlChart>
                            </div>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn DataField="office_id" UniqueName="TSRecordsProgressChartColumn" HeaderText="Progress For Time-Series Records">
                        <ItemTemplate>
                            <div style="width: 400px; height: 170px;">
                                <telerik:RadHtmlChart ID="rhcTSProgress" runat="server" Width="400" Height="170" Skin="Bootstrap">
                                    <Legend>
                                        <Appearance BackgroundColor="Transparent" Position="Bottom"></Appearance>
                                    </Legend>
                                    <ChartTitle>
                                        <Appearance Visible="false"></Appearance>
                                    </ChartTitle>
                                    <PlotArea>
                                        <Series>
                                            <telerik:BarSeries Name="Analyzed" Stacked="false" Gap="1.5" Spacing="0.4" DataFieldY="PercentTSAnalyzed">
                                                <Appearance>
                                                    <FillStyle BackgroundColor="#c5d291"></FillStyle>
                                                </Appearance>
                                                <LabelsAppearance Visible="false"></LabelsAppearance>
                                                <TooltipsAppearance BackgroundColor="#b84626" Color="White">
                                                    <ClientTemplate>
                                                        #=dataItem.PercentTSAnalyzedString# | #=dataItem.TSAnalyzed# records have been analyzed out of #=dataItem.TSRecords#
                                                    </ClientTemplate>
                                                </TooltipsAppearance>
                                            </telerik:BarSeries>
                                            <telerik:BarSeries Name="Approved" Stacked="false" Gap="1.5" Spacing="0.4" DataFieldY="PercentTSApproved">
                                                <Appearance>
                                                    <FillStyle BackgroundColor="#5cb8e3"></FillStyle>
                                                </Appearance>
                                                <LabelsAppearance Visible="False"></LabelsAppearance>
                                                <TooltipsAppearance BackgroundColor="#b84626" Color="White">
                                                    <ClientTemplate>
                                                        #=dataItem.PercentTSApprovedString# | #=dataItem.TSApproved# records have been approved out of #=dataItem.TSRecords#
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
                                </telerik:RadHtmlChart>
                            </div>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn DataField="office_id" UniqueName="NTSRecordsProgressChartColumn" HeaderText="Progress For Non-Time-Series Records">
                        <ItemTemplate>
                            <div style="width: 300px; height: 170px;">
                                <telerik:RadHtmlChart ID="rhcNTSProgress" runat="server" Width="300" Height="170" Skin="Bootstrap">
                                    <Legend>
                                        <Appearance BackgroundColor="Transparent" Position="Bottom"></Appearance>
                                    </Legend>
                                    <ChartTitle>
                                        <Appearance Visible="false"></Appearance>
                                    </ChartTitle>
                                    <PlotArea>
                                        <Series>
                                            <telerik:BarSeries Name="Analyzed" Stacked="false" Gap="1.5" Spacing="0.4" DataFieldY="PercentNTSAnalyzed">
                                                <Appearance>
                                                    <FillStyle BackgroundColor="#c5d291"></FillStyle>
                                                </Appearance>
                                                <LabelsAppearance Visible="false"></LabelsAppearance>
                                                <TooltipsAppearance BackgroundColor="#b84626" Color="White">
                                                    <ClientTemplate>
                                                        #=dataItem.PercentNTSAnalyzedString# | #=dataItem.NTSAnalyzed# records have been analyzed out of #=dataItem.NTSRecords#
                                                    </ClientTemplate>
                                                </TooltipsAppearance>
                                            </telerik:BarSeries>
                                            <telerik:BarSeries Name="Approved" Stacked="false" Gap="1.5" Spacing="0.4" DataFieldY="PercentNTSApproved">
                                                <Appearance>
                                                    <FillStyle BackgroundColor="#5cb8e3"></FillStyle>
                                                </Appearance>
                                                <LabelsAppearance Visible="False"></LabelsAppearance>
                                                <TooltipsAppearance BackgroundColor="#b84626" Color="White">
                                                    <ClientTemplate>
                                                        #=dataItem.PercentNTSApprovedString# | #=dataItem.NTSApproved# records have been approved out of #=dataItem.NTSRecords#
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
                                </telerik:RadHtmlChart>
                            </div>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
            <PagerStyle Mode="NextPrevAndNumeric"></PagerStyle>
        </telerik:RadGrid>
    </div>
</asp:Content>
