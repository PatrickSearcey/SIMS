<%@ Page Title="" Language="C#" MasterPageFile="~/RMSSingleMenu.Master" AutoEventWireup="true" CodeBehind="RecentActions.aspx.cs" Inherits="RMS.Report.RecentActions" %>
<%@ Register Src="~/Control/RecordPageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../styles/crpstatus.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <telerik:RadAjaxManager ID="ram" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="rrblDateRange">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlDateOptions" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbSubmit">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="ltlNumberOfRecords" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="rgRecentActions" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rgRecentActions">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgRecentActions" LoadingPanelID="ralp" />
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
                Select the desired office and date range to pull for the report.
            </p>
            <table width="1000">
                <tr>
                    <td colspan="4">
                        <telerik:RadDropDownList ID="rddlOffice" runat="server" DataValueField="office_id" DataTextField="office_nm" Skin="Bootstrap" Width="350px" DropDownHeight="200px" />
                    </td>
                </tr>
                <tr>
                    <td>Choose to view actions since last: 
                        <telerik:RadRadioButtonList ID="rrblDateRange" runat="server" Direction="Horizontal" AutoPostBack="true" OnSelectedIndexChanged="rrblDateRange_SelectedIndexChanged">
                            <Items>
                                <telerik:ButtonListItem Text="Day" Value="day" />
                                <telerik:ButtonListItem Text="Week" Value="week" />
                                <telerik:ButtonListItem Text="Month" Value="month" />
                            </Items>
                        </telerik:RadRadioButtonList>
                    </td>
                    <td>
                        <asp:Panel ID="pnlDateOptions" runat="server">
                            - OR - 
                            <telerik:RadDatePicker ID="rdpStartDt" runat="server" Skin="Bootstrap" DateInput-EmptyMessage="begin date" Width="200px" /> - to - 
                            <telerik:RadDatePicker ID="rdpEndDt" runat="server" Skin="Bootstrap" DateInput-EmptyMessage="end date" Width="200px" />
                        </asp:Panel>
                    </td>
                    <td><telerik:RadButton ID="rbSubmit" runat="server" OnCommand="UpdateGrid" CommandArgument="Update" Text="Retrieve" AutoPostBack="true" Skin="Bootstrap" /></td>
                    <td><asp:Literal ID="ltlError" runat="server" /></td>
                </tr>
                <tr>
                    <td colspan="4"><span class="filtersSubtext"><asp:Literal ID="ltlNumberOfRecords" runat="server" /></span></td>
                </tr>
            </table>
        </div>
        <hr />
        <telerik:RadGrid ID="rgRecentActions" runat="server" 
            OnNeedDataSource="rgRecentActions_NeedDataSource" 
            OnItemDataBound="rgRecentActions_ItemDataBound"  OnPreRender="rgRecentActions_PreRender"
            Skin="Bootstrap" 
            RenderMode="Lightweight" AllowPaging="false">
            <GroupingSettings CaseSensitive="false" />
            <MasterTableView DataKeyNames="site_id" AllowSorting="true" AllowFilteringByColumn="true" AutoGenerateColumns="false">
                <Columns>
                    <telerik:GridBoundColumn DataField="dialog_by" HeaderText="User" FilterControlWidth="60px" HeaderStyle-Width="70px" UniqueName="dialog_by" />
                    <telerik:GridBoundColumn DataField="dialog_dt" DataFormatString="{0:MM/dd/yyyy}" AllowFiltering="false" UniqueName="dialog_dt" HeaderText="When" />
                    <telerik:GridBoundColumn DataField="origin_va" UniqueName="origin_va" HeaderText="What" FilterControlWidth="60px" HeaderStyle-Width="70px" />
                    <telerik:GridBoundColumn DataField="status_set_to_va" UniqueName="status_set_to_va" HeaderText="Status Changed To" FilterControlWidth="60px" HeaderStyle-Width="70px" />
                    <telerik:GridTemplateColumn DataField="site_no" SortExpression="site_no" HeaderText="Number" FilterControlWidth="80px" ColumnGroupName="Site" UniqueName="site_no">
                        <ItemTemplate>
                            <a href='<%# String.Format("{0}StationInfo.aspx?site_id={1}", Eval("SIMSURL"), Eval("site_id")) %>' target="_blank"><%# Eval("site_no") %></a>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn DataField="station_nm" HeaderText="Name" FilterControlWidth="100px" ColumnGroupName="Site" UniqueName="station_nm" />
                    <telerik:GridTemplateColumn DataField="type_cd" SortExpression="type_cd" HeaderText="Record-Type" FilterControlWidth="80px" UniqueName="type_cd">
                        <ItemTemplate>
                            <asp:Literal ID="ltlTypeCd" runat="server" Text='<%# Eval("type_cd") %>' />
                            <telerik:RadToolTip RenderMode="Lightweight" ID="rttType" runat="server" TargetControlID="ltlTypeCd" RelativeTo="Element"
                                Position="BottomCenter" RenderInPageRoot="true">
                                <%# DataBinder.Eval(Container, "DataItem.type_ds") %>
                            </telerik:RadToolTip>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn DataField="period_beg_dt" SortExpression="period_beg_dt" HeaderText="Period" AllowFiltering="false" UniqueName="period">
                        <ItemTemplate>
                            <asp:Literal ID="ltlPeriodBegDt" runat="server" Text='<%# String.Format("{0:MM/dd/yyyy}", Eval("period_beg_dt")) %>' />-<asp:Literal ID="ltlPeriodEndDt" runat="server" Text='<%# String.Format("{0:MM/dd/yyyy}", Eval("period_end_dt")) %>' />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
    </div>
</asp:Content>
