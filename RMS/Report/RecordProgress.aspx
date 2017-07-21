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
                    <telerik:AjaxUpdatedControl ControlID="rhcAllRecords" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="rhcTSRecords" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="rhcNTSRecords" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="ltlAllRecordsTR" />
                    <telerik:AjaxUpdatedControl ControlID="ltlTSRecordsTR" />
                    <telerik:AjaxUpdatedControl ControlID="ltlNTSRecordsTR" />
                    <telerik:AjaxUpdatedControl ControlID="rgEmployees" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="rgRecordTypes" LoadingPanelID="ralp" />
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

        <telerik:RadTabStrip runat="server" ID="rts1" Orientation="HorizontalTop" SelectedIndex="0" MultiPageID="rmp1" Skin="Bootstrap">
            <Tabs>
                <telerik:RadTab Text="Breakdown By Technician" SelectedCssClass="selectedTab" SelectedIndex="0" />
                <telerik:RadTab Text="Breakdown By Record-Type" SelectedCssClass="selectedTab" />
            </Tabs>
        </telerik:RadTabStrip><telerik:RadMultiPage ID="rmp1" runat="server" SelectedIndex="0" Width="100%" CssClass="multiPage">
            <telerik:RadPageView runat="server" ID="rpv0">
                <telerik:RadGrid RenderMode="Lightweight" ID="rgEmployees" runat="server" GridLines="None" Skin="Bootstrap" OnNeedDataSource="rgEmployees_NeedDataSource"
                    OnItemDataBound="rgEmployees_ItemDataBound" AllowPaging="False" PageSize="10" AllowSorting="True">
                    <MasterTableView AutoGenerateColumns="False" DataKeyNames="user_id">
                        <Columns>
                            <telerik:GridBoundColumn DataField="user_id" HeaderText="Technician" SortExpression="user_id" UniqueName="user_id" />
                            <telerik:GridBoundColumn DataField="office_cd" HeaderText="Office Code" SortExpression="office_cd" UniqueName="office_cd" />
                            <telerik:GridTemplateColumn DataField="user_id" UniqueName="TotalProgressChartColumn" HeaderText="Total Progress on Tasks">
                                <ItemTemplate>
                                    <div style="width: 300px; height: 170px;">
                                        <telerik:RadHtmlChart ID="rhcTotalProgress" runat="server" Width="300" Height="170" Skin="Bootstrap">
                                            <Legend>
                                                <Appearance BackgroundColor="Transparent" Position="Bottom"></Appearance>
                                            </Legend>
                                            <ChartTitle>
                                                <Appearance Visible="false"></Appearance>
                                            </ChartTitle>
                                            <PlotArea>
                                                <Series>
                                                    <telerik:BarSeries Name="Analyzing" Stacked="false" Gap="1.5" Spacing="0.4" DataFieldY="PercentActuallyAnalyzed">
                                                        <Appearance>
                                                            <FillStyle BackgroundColor="#c5d291"></FillStyle>
                                                        </Appearance>
                                                        <LabelsAppearance Visible="false"></LabelsAppearance>
                                                        <TooltipsAppearance BackgroundColor="#b84626" Color="White">
                                                            <ClientTemplate>
                                                                #=dataItem.PercentActuallyAnalyzedString# | This field tech is the assigned analyst for #=dataItem.TotalRecordsAssignedToAnalyze# records; They have actually analyzed a total of #=dataItem.TotalActuallyAnalyzed# records.
                                                            </ClientTemplate>
                                                        </TooltipsAppearance>
                                                    </telerik:BarSeries>
                                                    <telerik:BarSeries Name="Approved" Stacked="false" Gap="1.5" Spacing="0.4" DataFieldY="PercentActuallyApproved">
                                                        <Appearance>
                                                            <FillStyle BackgroundColor="#5cb8e3"></FillStyle>
                                                        </Appearance>
                                                        <LabelsAppearance Visible="False"></LabelsAppearance>
                                                        <TooltipsAppearance BackgroundColor="#b84626" Color="White">
                                                            <ClientTemplate>
                                                                #=dataItem.PercentActuallyApprovedString# | This field tech is the assigned approver for #=dataItem.TotalRecordsAssignedToApprove# records; They have actually approved a total of #=dataItem.TotalActuallyApproved# records.
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
                            <telerik:GridTemplateColumn DataField="user_id" UniqueName="AssignedProgressChartColumn" HeaderText="Progress on Assigned Tasks">
                                <ItemTemplate>
                                    <div style="width: 300px; height: 170px;">
                                        <telerik:RadHtmlChart ID="rhcAssignedProgress" runat="server" Width="300" Height="170" Skin="Bootstrap">
                                            <Legend>
                                                <Appearance BackgroundColor="Transparent" Position="Bottom"></Appearance>
                                            </Legend>
                                            <ChartTitle>
                                                <Appearance Visible="false"></Appearance>
                                            </ChartTitle>
                                            <PlotArea>
                                                <Series>
                                                    <telerik:BarSeries Name="Analyzing" Stacked="false" Gap="1.5" Spacing="0.4" DataFieldY="PercentAssignedAnalyzed">
                                                        <Appearance>
                                                            <FillStyle BackgroundColor="#c5d291"></FillStyle>
                                                        </Appearance>
                                                        <LabelsAppearance Visible="false"></LabelsAppearance>
                                                        <TooltipsAppearance BackgroundColor="#b84626" Color="White">
                                                            <ClientTemplate>
                                                                #=dataItem.PercentAssignedAnalyzedString# | Of the #=dataItem.TotalRecordsAssignedToAnalyze# records for which this field tech is the assigned analyst, #=dataItem.TotalAssignedAnalyzed# have been analyzed. 
                                                            </ClientTemplate>
                                                        </TooltipsAppearance>
                                                    </telerik:BarSeries>
                                                    <telerik:BarSeries Name="Approving" Stacked="false" Gap="1.5" Spacing="0.4" DataFieldY="PercentAssignedApproved">
                                                        <Appearance>
                                                            <FillStyle BackgroundColor="#5cb8e3"></FillStyle>
                                                        </Appearance>
                                                        <LabelsAppearance Visible="False"></LabelsAppearance>
                                                        <TooltipsAppearance BackgroundColor="#b84626" Color="White">
                                                            <ClientTemplate>
                                                                #=dataItem.PercentAssignedApprovedString# | Of the #=dataItem.TotalRecordsAssignedToApprove# records for which this field tech is the assigned approver, #=dataItem.TotalAssignedApproved# have been approved.
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
                            <telerik:GridTemplateColumn DataField="user_id" UniqueName="AssignedForAnalyzingChartColumn" HeaderText="Records Assigned for Analyzing">
                                <ItemTemplate>
                                    <div style="width: 300px; height: 170px;">
                                        <telerik:RadHtmlChart ID="rhcAssignedForAnalyzing" runat="server" Width="300" Height="170" Skin="Bootstrap">
                                            <Legend>
                                                <Appearance BackgroundColor="Transparent" Position="Bottom"></Appearance>
                                            </Legend>
                                            <ChartTitle>
                                                <Appearance Visible="false"></Appearance>
                                            </ChartTitle>
                                            <PlotArea>
                                                <Series>
                                                    <telerik:BarSeries Name="Analyzed" Stacked="false" Gap="1.5" Spacing="0.4" DataFieldY="PercentRecordsAssignedToAnalyzeAnalyzed">
                                                        <Appearance>
                                                            <FillStyle BackgroundColor="#c5d291"></FillStyle>
                                                        </Appearance>
                                                        <LabelsAppearance Visible="false"></LabelsAppearance>
                                                        <TooltipsAppearance BackgroundColor="#b84626" Color="White">
                                                            <ClientTemplate>
                                                                #=dataItem.PercentRecordsAssignedToAnalyzeAnalyzedString# | #=dataItem.RecordsAssignedToAnalyzeAnalyzed# records have been analyzed out of #=dataItem.TotalRecordsAssignedToAnalyze# records assigned to analyze.
                                                            </ClientTemplate>
                                                        </TooltipsAppearance>
                                                    </telerik:BarSeries>
                                                    <telerik:BarSeries Name="Approved" Stacked="false" Gap="1.5" Spacing="0.4" DataFieldY="PercentRecordsAssignedToAnalyzeApproved">
                                                        <Appearance>
                                                            <FillStyle BackgroundColor="#5cb8e3"></FillStyle>
                                                        </Appearance>
                                                        <LabelsAppearance Visible="False"></LabelsAppearance>
                                                        <TooltipsAppearance BackgroundColor="#b84626" Color="White">
                                                            <ClientTemplate>
                                                                #=dataItem.PercentRecordsAssignedToAnalyzeApprovedString# | #=dataItem.RecordsAssignedToAnalyzeApproved# records have been approved out of #=dataItem.TotalRecordsAssignedToAnalyze# records assigned to analyze.
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
                            <telerik:GridTemplateColumn DataField="user_id" UniqueName="AssignedForApprovingChartColumn" HeaderText="Records Assigned for Approving">
                                <ItemTemplate>
                                    <div style="width: 300px; height: 170px;">
                                        <telerik:RadHtmlChart ID="rhcAssignedForApproving" runat="server" Width="300" Height="170" Skin="Bootstrap">
                                            <Legend>
                                                <Appearance BackgroundColor="Transparent" Position="Bottom"></Appearance>
                                            </Legend>
                                            <ChartTitle>
                                                <Appearance Visible="false"></Appearance>
                                            </ChartTitle>
                                            <PlotArea>
                                                <Series>
                                                    <telerik:BarSeries Name="Analyzed" Stacked="false" Gap="1.5" Spacing="0.4" DataFieldY="PercentRecordsAssignedToApproveAnalyzed">
                                                        <Appearance>
                                                            <FillStyle BackgroundColor="#c5d291"></FillStyle>
                                                        </Appearance>
                                                        <LabelsAppearance Visible="false"></LabelsAppearance>
                                                        <TooltipsAppearance BackgroundColor="#b84626" Color="White">
                                                            <ClientTemplate>
                                                                #=dataItem.PercentRecordsAssignedToApproveAnalyzedString# | #=dataItem.RecordsAssignedToApproveAnalyzed# records have been analyzed out of #=dataItem.TotalRecordsAssignedToApprove# records assigned to approve.
                                                            </ClientTemplate>
                                                        </TooltipsAppearance>
                                                    </telerik:BarSeries>
                                                    <telerik:BarSeries Name="Approved" Stacked="false" Gap="1.5" Spacing="0.4" DataFieldY="PercentRecordsAssignedToApproveApproved">
                                                        <Appearance>
                                                            <FillStyle BackgroundColor="#5cb8e3"></FillStyle>
                                                        </Appearance>
                                                        <LabelsAppearance Visible="False"></LabelsAppearance>
                                                        <TooltipsAppearance BackgroundColor="#b84626" Color="White">
                                                            <ClientTemplate>
                                                                #=dataItem.PercentRecordsAssignedToApproveApprovedString# | #=dataItem.RecordsAssignedToApproveApproved# records have been approved out of #=dataItem.TotalRecordsAssignedToApprove# records assigned to approve.
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
            </telerik:RadPageView>
            <telerik:RadPageView runat="server" ID="rpv1">
                <telerik:RadGrid RenderMode="Lightweight" ID="rgRecordTypes" runat="server" GridLines="None" Skin="Bootstrap" OnNeedDataSource="rgRecordTypes_NeedDataSource"
                    OnItemDataBound="rgRecordTypes_ItemDataBound" AllowPaging="False" PageSize="10" AllowSorting="True">
                    <MasterTableView AutoGenerateColumns="False" DataKeyNames="record_type_id">
                        <Columns>
                            <telerik:GridBoundColumn DataField="RecordType" HeaderText="Record-Type" SortExpression="type_ds" UniqueName="RecordType" />
                            <telerik:GridTemplateColumn DataField="record_type_id" UniqueName="ProgressChartColumn" HeaderText="Current Record Progress">
                                <ItemTemplate>
                                    <div style="width: 400px; height: 170px;">
                                        <telerik:RadHtmlChart ID="rhcProgress" runat="server" Width="400" Height="170" Skin="Bootstrap">
                                            <Legend>
                                                <Appearance BackgroundColor="Transparent" Position="Bottom"></Appearance>
                                            </Legend>
                                            <ChartTitle>
                                                <Appearance Visible="false"></Appearance>
                                            </ChartTitle>
                                            <PlotArea>
                                                <Series>
                                                    <telerik:BarSeries Name="Analyzed" Stacked="false" Gap="1.5" Spacing="0.4" DataFieldY="PercentAnalyzed">
                                                        <Appearance>
                                                            <FillStyle BackgroundColor="#c5d291"></FillStyle>
                                                        </Appearance>
                                                        <LabelsAppearance Visible="false"></LabelsAppearance>
                                                        <TooltipsAppearance BackgroundColor="#b84626" Color="White">
                                                            <ClientTemplate>
                                                                #=dataItem.PercentAnalyzedString# | #=dataItem.Analyzed# records analyzed of #=dataItem.TotalRecords# total records.
                                                            </ClientTemplate>
                                                        </TooltipsAppearance>
                                                    </telerik:BarSeries>
                                                    <telerik:BarSeries Name="Approved" Stacked="false" Gap="1.5" Spacing="0.4" DataFieldY="PercentApproved">
                                                        <Appearance>
                                                            <FillStyle BackgroundColor="#5cb8e3"></FillStyle>
                                                        </Appearance>
                                                        <LabelsAppearance Visible="False"></LabelsAppearance>
                                                        <TooltipsAppearance BackgroundColor="#b84626" Color="White">
                                                            <ClientTemplate>
                                                                #=dataItem.PercentApprovedString# | #=dataItem.Approved# records approved of #=dataItem.TotalRecords# total records.
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
            </telerik:RadPageView>
        </telerik:RadMultiPage>
    </div>
</asp:Content>
