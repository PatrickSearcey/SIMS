<%@ Page Title="" Language="C#" MasterPageFile="~/RMSSingleMenu.Master" AutoEventWireup="true" CodeBehind="PeriodDetails.aspx.cs" Inherits="RMS.Report.PeriodDetails" %>
<%@ Register Src="~/Control/RecordPageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../styles/perioddetails.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <telerik:RadAjaxManager ID="ram" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="rbSubmit">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rmp1" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="dlOuterSANAL" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="dlOuterChangeLogs" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="dlOuterDialogs" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="ltlNotice1" />
                    <telerik:AjaxUpdatedControl ControlID="ltlNotice2" />
                    <telerik:AjaxUpdatedControl ControlID="ltlNotice3" />
                    <telerik:AjaxUpdatedControl ControlID="ltlError" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rddlOffice">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rddlRecords" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="rbSubmit" />
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
                You may choose to retrieve details for a given time period for all records in an office, or by individual record.
            </p>
            <table>
                <tr>
                    <td><telerik:RadDropDownList ID="rddlOffice" runat="server" DataValueField="office_id" DataTextField="office_nm" AutoPostBack="true" OnSelectedIndexChanged="UpdateRecords" 
                        Skin="Bootstrap" Width="350px" DropDownHeight="200px" /></td>
                    <td colspan="2"><telerik:RadDropDownList ID="rddlRecords" runat="server" DataValueField="rms_record_id" DataTextField="SiteRecord" Skin="Bootstrap" Width="500px" DropDownHeight="500px" /></td>
                </tr>
                <tr>
                    <td><telerik:RadDatePicker ID="rdpBeginDt" runat="server" Skin="Bootstrap" DateInput-EmptyMessage="begin date" Width="150px" /> - to - 
                        <telerik:RadDatePicker ID="rdpEndDt" runat="server" Skin="Bootstrap" DateInput-EmptyMessage="end date" Width="150px" /></td>
                    <td><telerik:RadButton ID="rbSubmit" runat="server" OnCommand="UpdateDetails" CommandArgument="Update" Text="Submit" AutoPostBack="true" Skin="Bootstrap" /></td>
                    <td><asp:Literal ID="ltlError" runat="server" /></td>
                </tr>
            </table>
        </div>
        <telerik:RadTabStrip runat="server" ID="rts1" Orientation="HorizontalTop" SelectedIndex="0" MultiPageID="rmp1" Skin="Bootstrap">
            <Tabs>
                <telerik:RadTab Text="Station Analyses" SelectedCssClass="selectedTab" SelectedIndex="0" />
                <telerik:RadTab Text="Change Logs" SelectedCssClass="selectedTab" />
                <telerik:RadTab Text="Dialogs" SelectedCssClass="selectedTab" />
            </Tabs>
        </telerik:RadTabStrip><telerik:RadMultiPage ID="rmp1" runat="server" SelectedIndex="0" Width="100%" CssClass="multiPage">
            <telerik:RadPageView runat="server" ID="rpv0">
                <p style="font-weight:bold; padding: 0 5px 0 5px;">
                    <asp:Literal ID="ltlNotice1" runat="server">Please use the filters above and click the submit button to retrieve Station Analyses.</asp:Literal>

                    <asp:DataList ID="dlOuterSANAL" runat="server" OnItemDataBound="dlOuterSANAL_ItemDataBound">
                        <ItemTemplate>
                            <asp:HiddenField ID="hfRecordID" runat="server" Value='<%# Eval("rms_record_id") %>' />
                            <div style="text-align:center;font-weight:bold;font-size:12pt;width:100%">
                                <%# Eval("site_no") %> <%# Eval("station_full_nm") %><br />
                                <%# Eval("type_ds") %>
                            </div>
                            <asp:DataList ID="dlInnerSANAL" runat="server">
                                <ItemTemplate>
                                    <hr />
                                    <div style="padding-left:10px;">
                                        <p style="font-weight:bold;">Analysis Period: <%# Eval("timespan") %></p>
                                        <p style="font-weight:bold;">Analysis:</p>
                                        <div style="width:1200px;overflow-x:scroll;background-color:white;padding-left:5px;">
                                            <%# Eval("analysis_notes_va") %>
                                        </div>
                                        <p style="font-style:italic;">Analysis for this period last updated <%# Eval("edited_dt") %> by <%# Eval("edited_by_uid") %></p>
                                        <table border="0" width="650px">
                                            <tr>
                                                <td align="right"><span style="font-style:italic;">Analyzed By: </span></td>
                                                <td><%# Eval("analyzed_by") %></td>
                                                <td align="right"><span style="font-style:italic;">Approved By: </span></td>
                                                <td><%# Eval("approved_by") %></td>
                                            </tr>
                                            <tr>
                                                <td align="right"><span style="font-style:italic;">Date:</span></td>
                                                <td><%# Eval("analyzed_dt") %></td>
                                                <td align="right"><span style="font-style:italic;">Date:</span></td>
                                                <td><%# Eval("approved_dt") %></td>
                                            </tr>
                                        </table>
                                    </div>
                                </ItemTemplate>
                            </asp:DataList>
                            <hr />
                        </ItemTemplate>
                    </asp:DataList>
                </p>
            </telerik:RadPageView>

            <telerik:RadPageView runat="server" ID="rpv1">
                <p style="font-weight:bold;padding: 0 5px 0 5px;">
                    <asp:Literal ID="ltlNotice2" runat="server">Please use the filters above and click the submit button to retrieve Change Logs.</asp:Literal>

                    <asp:DataList ID="dlOuterChangeLogs" runat="server" OnItemDataBound="dlOuterChangeLogs_ItemDataBound" Width="100%">
                        <ItemTemplate>
                            <asp:HiddenField ID="hfRecordID" runat="server" Value='<%# Eval("rms_record_id") %>' />
                            <div style="text-align:center;font-weight:bold;font-size:12pt;width:100%">
                                <%# Eval("site_no") %> <%# Eval("station_full_nm") %><br />
                                <%# Eval("type_ds") %>
                            </div>
                            <asp:DataList ID="dlInnerChangeLogs" runat="server" OnItemDataBound="dlInnerChangeLogs_ItemDataBound" Width="100%">
                                <ItemTemplate>
                                    <hr />
                                    <div style="padding-left:10px;">
                                        <asp:HiddenField ID="hfPeriodID" runat="server" Value='<%# Eval("period_id") %>' />
                                        <p style="font-weight:bold;">Analysis Period: <%# Eval("timespan") %></p>
                                        <p style="font-weight:bold;">Analysis Period Change Log:</p>
                                        <div style="width:1200px;overflow-x:scroll;">
                                            <asp:DataGrid ID="dgChangeLog" runat="server" AutoGenerateColumns="false" Width="100%" BackColor="White" CellPadding="5" BorderColor="Wheat">
                                                <Columns>
                                                    <asp:BoundColumn DataField="edited_dt" HeaderText="Date and Time" ItemStyle-Width="180px" ItemStyle-VerticalAlign="Top" />
                                                    <asp:BoundColumn DataField="edited_by_uid" HeaderText="User ID" ItemStyle-Width="80px" ItemStyle-VerticalAlign="Top" />
                                                    <asp:BoundColumn DataField="new_va" HeaderText="Analysis" />
                                                </Columns>
                                                <HeaderStyle Font-Bold="true" CssClass="Header" />
                                            </asp:DataGrid>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:DataList>
                            <hr />
                        </ItemTemplate>
                    </asp:DataList>
                </p>
            </telerik:RadPageView>

            <telerik:RadPageView runat="server" ID="rpv2">
                <p style="font-weight:bold;padding: 0 5px 0 5px;">
                    <asp:Literal ID="ltlNotice3" runat="server">Please use the filters above and click the submit button to retrieve Dialogs.</asp:Literal>

                    <asp:DataList ID="dlOuterDialogs" runat="server" OnItemDataBound="dlOuterDialogs_ItemDataBound" widh="100%">
                        <ItemTemplate>
                            <asp:HiddenField ID="hfRecordID" runat="server" Value='<%# Eval("rms_record_id") %>' />
                            <div style="text-align:center;font-weight:bold;font-size:12pt;width:100%">
                                <%# Eval("site_no") %> <%# Eval("station_full_nm") %><br />
                                <%# Eval("type_ds") %>
                            </div>
                            <asp:DataList ID="dlInnerDialogs" runat="server" OnItemDataBound="dlInnerDialogs_ItemDataBound" Width="100%">
                                <ItemTemplate>
                                    <hr />
                                    <div style="padding-left:10px;">
                                        <asp:HiddenField ID="hfPeriodID" runat="server" Value='<%# Eval("period_id") %>' />
                                        <p style="font-weight:bold;">Analysis Period: <%# Eval("timespan") %></p>
                                        <p style="font-weight:bold;">Analysis Period Dialog:</p>
                                        <asp:DataGrid ID="dgDialog" runat="server" AutoGenerateColumns="false" Width="95%" BackColor="White" CellPadding="5" BorderColor="Wheat">
                                            <Columns>
                                                <asp:BoundColumn DataField="dialog_dt" HeaderText="Date and Time" ItemStyle-Width="180px" />
                                                <asp:BoundColumn DataField="origin_va" HeaderText="From" ItemStyle-Width="80px" />
                                                <asp:BoundColumn DataField="dialog_by" HeaderText="User ID" ItemStyle-Width="80px" />
                                                <asp:BoundColumn DataField="status_set_to_va" HeaderText="Status Set To" ItemStyle-Width="120px" />
                                                <asp:BoundColumn DataField="comments_va" HeaderText="Comments" />
                                            </Columns>
                                            <HeaderStyle Font-Bold="true" CssClass="Header" />
                                        </asp:DataGrid>
                                    </div>
                                </ItemTemplate>
                            </asp:DataList>
                            <hr />
                        </ItemTemplate>
                    </asp:DataList>
                </p>
            </telerik:RadPageView>
        </telerik:RadMultiPage>
    </div>
</asp:Content>
