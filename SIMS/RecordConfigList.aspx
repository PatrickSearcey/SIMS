<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/RMS.Master" CodeBehind="RecordConfigList.aspx.vb" Inherits="SIMS.RecordConfigList" %>
<%@ MasterType  virtualPath="~/RMS.master" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxtoolkit" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<asp:Content ID="c1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="c2" ContentPlaceHolderID="cph1" Runat="Server">
<asp:Panel ID="pnlHasAccess" runat="server">
    <telerik:RadAjaxPanel ID="rap1" runat="server">
        <table width="100%">
            <tr>
                <td valign="top">
                    Choose a WSC: <br />
                    <asp:DropDownList ID="ddlWSC" runat="server" OnSelectedIndexChanged="ddlWSC_SelectedIndexChanged"
                        DataTextField="wsc_nm" DataValueField="wsc_id" AutoPostBack="true" />
                    <asp:Panel ID="pnlOffice" runat="server" Visible="false">
                        <br />Choose an Office: <br />
                        <asp:DropDownList ID="ddlOffice" runat="server" AutoPostBack="true" onSelectedIndexChanged="ddlOffice_SelectedIndexChanged"
                            DataTextField="office_nm" DataValueField="office_id" />
                    </asp:Panel>
                    <asp:Panel ID="pnlOnlyActive" runat="server" Visible="false"><br />Currently viewing: <asp:RadioButtonList ID="rblOnlyActive" runat="server" AutoPostBack="true" 
                            RepeatDirection="Horizontal" OnSelectedIndexChanged="rblOnlyActive_SelectedIndexChanged">
                        <asp:ListItem Value="yes">only active records</asp:ListItem>
                        <asp:ListItem Value="no">all records</asp:ListItem>
                    </asp:RadioButtonList></asp:Panel>
                </td>
                <td valign="top">
                    <asp:Panel ID="pnlExplanation" runat="server">
                        <fieldset style="width:700px;height:205px;">
                            <legend>Explanation</legend>
                            <div style="padding:5px;">
                            <ul style="padding-top:-15px;margin-top:3px;">
                                <li><label>Clicking on the site number will open the Station Information page</label></li>
                                <li><label>Clicking on the record-type description will open the Record Configuration Interface</label></li>
                                <li><label>The <i>ts class</i> column shows the classification of the record type; <i>ts</i> means
                                the record has been assigned to a time-series record-type, and <i>nts</i> means the record has been assigned
                                to a non-time-series record-type.</label></li>
                                <li><label>The <i>cat no</i> column shows the assigned category number for the record. If the category
                                number is 2 or 3, the <i>cat reason</i> column displays the remarks for why the record was categorized this way.</label></li>
                                <li><label>The <i>DD & Parameter</i> column shows the DD and corresponding parameter code and name that has been
                                assigned to the record in RMS.  If more than one DD has been assigned to the record, <i>multi-parameter</i> is
                                displayed.</label></li>
                                <li><label>When viewing <i>all records</i>, rows with light orange backgrounds are inactive records.</label></li>
                                <li><label><b>Note: DD & Parameter code will not show in this interface until 
                                approved daily values are sent to NWISWeb. DD assignment can be seen on Records 
                                Configuration Interface.  Click on Record Type.</b></label></li>
                            </ul>
                            </div>
                        </fieldset>
                    </asp:Panel>
                </td>
            </tr>
        </table>
        <div align="right"><asp:Label runat="server" ID="lblRecordCount" CssClass="SIBodyFont" Visible="false" Font-Italic="true" /></div>
        <telerik:RadGrid ID="rgRecs" runat="server" Skin="Sunset" AllowFilteringByColumn="true" OnNeedDataSource="rgRecs_NeedDataSource"
            AllowSorting="true" AutoGenerateColumns="false" ShowStatusBar="true" EnableLinqExpressions="false">
            <MasterTableView>
                <Columns>
                    <telerik:GridTemplateColumn UniqueName="SiteNumber" HeaderText="site no" SortExpression="site_no"
                        ItemStyle-VerticalAlign="Top" DataField="site_no" FilterControlWidth="50">
                        <ItemTemplate>
                            <a href='/SIMSClassic/StationInfo.asp?site_no=<%#DataBinder.Eval(Container.DataItem, "site_no").ToString%>' target="_blank">
                            <%#DataBinder.Eval(Container.DataItem, "site_no")%></a>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn UniqueName="StationName" DataField="station_nm_short" ItemStyle-Wrap="false"
                        HeaderText="station nm" ItemStyle-VerticalAlign="Top" />
                    <telerik:GridBoundColumn UniqueName="Worker" DataField="operator_va" HeaderText="worker" FilterControlWidth="40" />
                    <telerik:GridBoundColumn UniqueName="Checker" DataField="assigned_checker_uid" HeaderText="checker" FilterControlWidth="40" />
                    <telerik:GridBoundColumn UniqueName="Reviewer" DataField="assigned_reviewer_uid" HeaderText="reviewer" FilterControlWidth="40" />
                    <telerik:GridTemplateColumn UniqueName="RecordType" HeaderText="record-type" SortExpression="type_ds" 
                        DataField="type_ds" ItemStyle-Wrap="false">
                        <ItemTemplate>
                            <a href='RecordConfig.aspx?rms_record_id=<%#DataBinder.Eval(Container.DataItem, "rms_record_id")%>'>
                            <%#DataBinder.Eval(Container.DataItem, "type_ds")%></a>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn UniqueName="TimeSeries" DataField="ts_fg" AllowFiltering="false" ItemStyle-HorizontalAlign="Center" 
                        HeaderText="ts class" />
                    <telerik:GridBoundColumn UniqueName="CategoryNo" DataField="category_no" HeaderText="cat no" FilterControlWidth="10" ItemStyle-HorizontalAlign="Center" />
                    <telerik:GridBoundColumn UniqueName="CategoryReason" DataField="cat_reason" AllowFiltering="false"
                        HeaderText="cat reason" />
                    <telerik:GridBoundColumn UniqueName="DDConfigs" DataField="dd_full_ds" HeaderText="DD & Parameter"  ItemStyle-Wrap="false"/>
                    <telerik:GridBoundColumn UniqueName="ActiveStatus" DataField="not_used_fg" Display="false" />
                    <telerik:GridBoundColumn UniqueName="StationNameFull" DataField="station_nm" Display="false" />
                </Columns>
                <HeaderStyle HorizontalAlign="Center" />
            </MasterTableView>
            <ClientSettings>
                <DataBinding EnableCaching="false" />
            </ClientSettings>
        </telerik:RadGrid>
    </telerik:RadAjaxPanel>
</asp:Panel>
</asp:Content>
