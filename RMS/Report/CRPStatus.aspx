<%@ Page Title="" Language="C#" MasterPageFile="~/RMSSingleMenu.Master" AutoEventWireup="true" CodeBehind="CRPStatus.aspx.cs" Inherits="RMS.Report.CRPStatus" %>
<%@ Register Src="~/Control/RecordPageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../styles/crpstatus.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
        <telerik:RadAjaxManager ID="ram" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="rbSubmit">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="ltlNumberOfRecords" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="rgCRPStatus" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rgCRPStatus">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgCRPStatus" LoadingPanelID="ralp" />
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
                Select the desired office and records to pull for the report.
            </p>
            <table width="1000">
                <tr>
                    <td><telerik:RadDropDownList ID="rddlOffice" runat="server" DataValueField="office_id" DataTextField="office_nm" Skin="Bootstrap" Width="350px" DropDownHeight="200px" /></td>
                    <td>Currently Viewing: 
                        <telerik:RadRadioButtonList ID="rrblRecords" runat="server" Direction="Horizontal" AutoPostBack="false">
                            <Items>
                                <telerik:ButtonListItem Text="only active records" Value="active" Selected="true" />
                                <telerik:ButtonListItem Text="all records" Value="all" />
                            </Items>
                        </telerik:RadRadioButtonList></td>
                    <td><telerik:RadButton ID="rbSubmit" runat="server" OnCommand="UpdateDetails" CommandArgument="Update" Text="Submit" AutoPostBack="true" Skin="Bootstrap" /></td>
                    <td><asp:Literal ID="ltlError" runat="server" /></td>
                </tr>
                <tr>
                    <td colspan="2"><span class="filtersSubtext"><asp:Literal ID="ltl150DaysAgo" runat="server" /> &nbsp;&nbsp;&nbsp;<asp:Literal ID="ltl240DaysAgo" runat="server" /></span></td>
                    <td colspan="2"><span class="filtersSubtext"><asp:Literal ID="ltlNumberOfRecords" runat="server" /></span></td>
                </tr>
            </table>
        </div>
        <telerik:RadPanelBar ID="rpbExplanation" runat="server" Width="100%" Skin="Bootstrap">
            <Items>
                <telerik:RadPanelItem Text="Explanation (click here to collapse/expand)" Expanded="false">
                    <ContentTemplate>
                        <div style="padding:10px;font-size:9pt;">
                            <div style="width:12px;height:12px;background-color:#eee8aa;border:solid 1px #808080;float:left;"></div> &nbsp;light yellow cell backgrounds in the 
                            analyze/approve period columns mean that the period was completed within the designated category guidelines (within the last 150 days for category 1 records, 
                            and within the last 240 days for category 2 records)<br />
                            <div style="width:12px;height:12px;background-color:#bc9f9f;border:solid 1px #808080;float:left;"></div> &nbsp;rose cell backgrounds denote last aging dates 
                            within category time limits<br />
                            <div style="width:12px;height:12px;background-color:#deb887;border:solid 1px #808080;float:left;"></div> &nbsp;peach cell backgrounds signify that all 
                            analyzed periods in RMS have been approved<br />
                            <div style="width:12px;height:12px;background-color:#bdb76b;border:solid 1px #808080;float:left;"></div> &nbsp;khaki cell backgrounds signify that all 
                            analyzed periods in RMS have been approved within the designated category guidelines (within the last 150 days for category 1 records, and within the last 
                            240 days for category 2 records)</p>
                            <b>Notes:</b>
                            <ul>
                                <li>Clicking on the site number will open the Station Information page</li>
                                <li>The cat no column shows the assigned category number for the record. If category number equal to 1, the last aging dates are highlighted based on being approved in ADAPS within the last 150 days. If category number equal to 2, the last aging dates are highlighted based on being approved in ADAPS within the last 240 days.</li>
                                <li>Clicking on the dates in the last analyzed and last approved period in RMS columns will open the corresponding record processing page</li>
                                <li>For cases where only provisional data exists for a TS ID, the LAST APPROVED DV IN AQ columns will be blank.</li>
                            </ul>
                        </div>
                    </ContentTemplate>
                </telerik:RadPanelItem>
            </Items>
        </telerik:RadPanelBar>
        <hr />
        <telerik:RadGrid ID="rgCRPStatus" runat="server" 
            OnNeedDataSource="rgCRPStatus_NeedDataSource" 
            OnItemDataBound="rgCRPStatus_ItemDataBound"  OnPreRender="rgCRPStatus_PreRender"
            Skin="Bootstrap" 
            RenderMode="Lightweight" AllowPaging="false">
            <MasterTableView DataKeyNames="rms_record_id" AllowSorting="true" AllowFilteringByColumn="true" AutoGenerateColumns="false">
                <ColumnGroups>
                    <telerik:GridColumnGroup HeaderText="Site" Name="Site" HeaderStyle-HorizontalAlign="Center" />
                    <telerik:GridColumnGroup HeaderText="Last Approved DV In AQ" Name="AQ" HeaderStyle-HorizontalAlign="Center" />
                </ColumnGroups>
                <Columns>
                    <telerik:GridBoundColumn DataField="office_cd" HeaderText="Office" AllowFiltering="false" HeaderStyle-Width="40px" UniqueName="office_cd" />
                    <telerik:GridTemplateColumn SortExpression="site_no" HeaderText="Number" FilterControlWidth="80px" ColumnGroupName="Site" UniqueName="site_no">
                        <ItemTemplate>
                            <a href='<%# String.Format("{0}StationInfo.aspx?site_id={1}", Eval("SIMS2017URL"), Eval("site_id")) %>' target="_blank"><%# Eval("site_no") %></a>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn DataField="station_nm" HeaderText="Name" FilterControlWidth="100px" ColumnGroupName="Site" UniqueName="station_nm" />
                    <telerik:GridBoundColumn DataField="parm_cd" HeaderText="Param Code" AllowFiltering="false" UniqueName="parm_cd" />
                    <telerik:GridTemplateColumn SortExpression="type_cd" HeaderText="Record-Type" FilterControlWidth="80px" UniqueName="type_cd">
                        <ItemTemplate>
                            <asp:Literal ID="ltlTypeCd" runat="server" Text='<%# Eval("type_cd") %>' />
                            <telerik:RadToolTip RenderMode="Lightweight" ID="rttType" runat="server" TargetControlID="ltlTypeCd" RelativeTo="Element"
                                Position="BottomCenter" RenderInPageRoot="true">
                                <%# DataBinder.Eval(Container, "DataItem.type_ds") %>
                            </telerik:RadToolTip>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn DataField="category_no" HeaderText="Cat. No." AllowFiltering="false" UniqueName="category_no" />
                    <telerik:GridTemplateColumn SortExpression="analyzed_period_beg_dt" HeaderText="Last Analyzed Period in RMS" AllowFiltering="false" UniqueName="analyzed_period_dt">
                        <ItemTemplate>
                            <a href='RecordProcess.aspx?task=analyze'><%# Eval("analyzed_period_dt") %></a>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn SortExpression="approved_period_beg_dt" HeaderText="Last Approved Period in RMS" AllowFiltering="false" UniqueName="approved_period_dt">
                        <ItemTemplate>
                            <a href='RecordProcess.aspx?task=approve'><%# Eval("approved_period_dt") %></a>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn DataField="last_aging_dt" DataFormatString="{0:MM/dd/yyyy}" AllowFiltering="false" UniqueName="last_aging_dt" HeaderText="Date" ColumnGroupName="AQ" />
                    <telerik:GridBoundColumn DataField="DaysSinceAging" AllowFiltering="false" UniqueName="DaysSinceAging" HeaderText="Days Ago" ColumnGroupName="AQ" />
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
    </div>
</asp:Content>
