<%@ Page Title="" Language="C#" MasterPageFile="~/SIMSSingleMenu.Master" AutoEventWireup="true" CodeBehind="Offices.aspx.cs" Inherits="SIMS2017.Admin.Offices" ValidateRequest="false" %>
<%@ Register Src="~/Control/SitePageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
        <link href="../styles/admin.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <telerik:RadAjaxManager ID="ram" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="rgOffices">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgOffices" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlNotice" />
                    <telerik:AjaxUpdatedControl ControlID="ltlError" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbUpdate">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="ltlError" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="ralp" runat="server" Skin="Bootstrap" />

    <uc:PageHeading id="ph1" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph2" runat="server">
    <div class="mainContent">
        <asp:Panel ID="pnlNoAccess" runat="server">
            <h4>You do not have the necessary permission to access Administration Tasks for this WSC. Please contact your WSC SIMS Admin if you require access.</h4>
        </asp:Panel>

        <asp:Panel ID="pnlHasAccess" runat="server">
            <asp:Panel ID="pnlNotice" runat="server" CssClass="pnlNotes">
                <h4><asp:Literal ID="ltlNoticeHeading" runat="server" /></h4>
                <asp:Literal ID="ltlNotice" runat="server" />
            </asp:Panel>

            <telerik:RadGrid ID="rgOffices" runat="server" AutoGenerateColumns="false" 
                Skin="Bootstrap" GridLines="None" ShowStatusBar="true" PageSize="50"
                AllowSorting="true" 
                AllowMultiRowSelection="false" 
                AllowFiltering="false"
                AllowPaging="false"
                AllowAutomaticDeletes="true"
                OnNeedDataSource="rgOffices_NeedDataSource"
                OnUpdateCommand="rgOffices_UpdateCommand" 
                OnItemDataBound="rgOffices_ItemDataBound"
                OnPreRender="rgOffices_PreRender">
                <PagerStyle Mode="NumericPages" />
                <MasterTableView DataKeyNames="office_id" AllowMultiColumnSorting="true" Width="100%" CommandItemDisplay="None" Name="Offices">
                    <Columns>
                        <telerik:GridEditCommandColumn ButtonType="ImageButton" UniqueName="EditCommandColumn">
                            <HeaderStyle Width="20px" />
                        </telerik:GridEditCommandColumn>
                        <telerik:GridBoundColumn DataField="office_id" UniqueName="office_id" Display="false" ReadOnly="true"  />
                        <telerik:GridBoundColumn DataField="office_cd" HeaderText="Office Code" UniqueName="office_cd" SortExpression="office_cd" HeaderStyle-Width="100px"  />
                        <telerik:GridBoundColumn DataField="office_nm" HeaderText="Office Name" UniqueName="office_nm" SortExpression="office_nm"  />
                    </Columns>

                    <EditFormSettings EditFormType="Template">
                        <FormTemplate>
                            <div style="padding:5px;background-color:#ebe2c0;font-size:10pt;">
                                <table id="tableForm1" cellspacing="0" cellpadding="2" width="800" border="0" rules="none" style="border-collapse: collapse; background: #ebe2c0;">
                                    <tr>
                                        <td colspan="2">
                                            <h4>Modify Office Details</h4>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width:200px;">
                                            <label>Office Code:</label>
                                        </td>
                                        <td>
                                            <asp:Literal ID="ltlOfficeCd" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label>Office Name:</label>
                                        </td>
                                        <td>
                                            <telerik:RadTextBox ID="rtbOfficeName" runat="server" Width="350px" />
                                            <asp:RequiredFieldValidator ID="rfvOfficeName" runat="server" ControlToValidate="rtbOfficeName" ErrorMessage="* required" ForeColor="Red" Font-Bold="true" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label>Street Address:</label>
                                        </td>
                                        <td>
                                            <telerik:RadTextBox ID="rtbStreetAddress" runat="server" Width="350px"  />
                                            <asp:RequiredFieldValidator ID="rfvStreetAddress" runat="server" ControlToValidate="rtbStreetAddress" ErrorMessage="* required" ForeColor="Red" Font-Bold="true" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label>City, St, Zip:</label>
                                        </td>
                                        <td>
                                            <telerik:RadTextBox ID="rtbCityStateZip" runat="server" Width="350px"  />
                                            <asp:RequiredFieldValidator ID="rfvCityStateZip" runat="server" ControlToValidate="rtbCityStateZip" ErrorMessage="* required" ForeColor="Red" Font-Bold="true" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label style="line-height:9pt;">Office Coordinates<br />(in decimal degrees):</label>
                                        </td>
                                        <td>
                                            Latitude <telerik:RadNumericTextBox ID="rntbLat" runat="server" Width="100px" NumberFormat-DecimalDigits="6" />
                                            Longitude <telerik:RadNumericTextBox ID="rntbLong" runat="server" Width="100px" NumberFormat-DecimalDigits="6" />
                                            <asp:Image ID="imgCoordinatesHelp" runat="server" ImageUrl="~/Images/tooltip.png" />
                                            <asp:RequiredFieldValidator ID="rfvLat" runat="server" ControlToValidate="rntbLat" ErrorMessage="*" ForeColor="Red" Font-Bold="true" />
                                            <asp:RequiredFieldValidator ID="rfvLong" runat="server" ControlToValidate="rntbLong" ErrorMessage="*" ForeColor="Red" Font-Bold="true" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label>Phone No.:</label>
                                        </td>
                                        <td>
                                            <telerik:RadTextBox ID="rtbPhoneNo" runat="server" Width="150px" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label>Data Chief E-mail:</label>
                                        </td>
                                        <td>
                                            <telerik:RadTextBox ID="rtbDataChiefEmail" runat="server" Width="300px" />
                                            <asp:Image ID="imgDataChiefEmail" runat="server" ImageUrl="~/Images/tooltip.png" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label>Approver E-mail:</label>
                                        </td>
                                        <td>
                                            <telerik:RadTextBox ID="rtbReviewerEmail" runat="server" Width="300px" />
                                            <asp:Image ID="imgReviewerEmail" runat="server" ImageUrl="~/Images/tooltip.png" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label>Subnet IP Address:</label>
                                        </td>
                                        <td>
                                            <asp:Literal ID="ltlIPAddress" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label>Time Zone Code:</label>
                                        </td>
                                        <td>
                                            <telerik:RadDropDownList ID="rddlTimeZone" runat="server" Width="80px">
                                                <Items>
                                                    <telerik:DropDownListItem Value="" Text="" />
                                                    <telerik:DropDownListItem Value="AST" Text="AST" />
                                                    <telerik:DropDownListItem Value="CST" Text="CST" />
                                                    <telerik:DropDownListItem Value="EST" Text="EST" />
                                                    <telerik:DropDownListItem Value="MST" Text="MST" />
                                                    <telerik:DropDownListItem Value="PST" Text="PST" />
                                                </Items>
                                            </telerik:RadDropDownList>
                                            <asp:RequiredFieldValidator ID="rfvTimeZone" runat="server" ControlToValidate="rddlTimeZone" ErrorMessage="* required" ForeColor="Red" Font-Bold="true" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label>Start New Record Option:</label>
                                        </td>
                                        <td>
                                            <telerik:RadRadioButtonList ID="rrblNewRecord" runat="server" Width="110px">
                                                <Items>
                                                    <telerik:ButtonListItem Value="No" Text="Do not allow" />
                                                    <telerik:ButtonListItem Value="Yes" Text="Allow" />
                                                </Items>
                                            </telerik:RadRadioButtonList>
                                            <asp:Image ID="imgNewRecord" runat="server" ImageUrl="~/Images/tooltip.png" CssClass="ImagePadding" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" colspan="2">
                                            <telerik:RadButton ID="rbUpdate" Text="Update" runat="server" CommandName="Update" Skin="Bootstrap" />
                                            <telerik:RadButton ID="rbCancel" Text="Cancel" runat="server" CausesValidation="false" CommandName="Cancel" Skin="Bootstrap" /><br />
                                            <asp:Literal ID="ltlError" runat="server" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <telerik:RadToolTip runat="server" ID="rtt1" RelativeTo="Element" Width="300px" AutoCloseDelay="10000"
                                Height="180px" TargetControlID="imgCoordinatesHelp" IsClientID="false" Animation="Fade" Position="TopRight">
                                Coordinates are for plotting your office on the field trip maps. Please enter coordinates only in decimal degrees (i.e. lat: 46.600026, long: -111.979589).<br /><br />
                                Visit http://geocoder.us/ to find coordinates based on an address. 
                            </telerik:RadToolTip>
                            <telerik:RadToolTip runat="server" ID="rrt2" RelativeTo="Element" Width="300px" AutoCloseDelay="10000"
                                Height="180px" TargetControlID="imgDataChiefEmail" IsClientID="false" Animation="Fade" Position="TopRight">
                                Please enter the full e-mail address of the data chief (i.e. medorsey@usgs.gov <b>or</b> Mike Dorsey &lt;medorsey@usgs.gov&gt;).<br /><br />For multiple e-mails, separate each by a comma (i.e. Mike Dorsey &lt;medorsey@usgs.gov&gt;, Craig Weiss &lt;kcweiss@usgs.gov&gt;).
                            </telerik:RadToolTip>
                            <telerik:RadToolTip runat="server" ID="rtt3" RelativeTo="Element" Width="300px" AutoCloseDelay="10000"
                                Height="200px" TargetControlID="imgReviewerEmail" IsClientID="false" Animation="Fade" Position="TopRight">
                                Please enter the full e-mail address (i.e. medorsey@usgs.gov <b>or</b> Mike Dorsey &lt;medorsey@usgs.gov&gt;) to which notification e-mails should be sent after a record period has been analyzed and is ready for approval.<br /><br />
                                Analyzed notification e-mails will be sent to <b>both the assigned approver and the e-mail address(es) entered in this field.</b>
                            </telerik:RadToolTip>
                            <telerik:RadToolTip runat="server" ID="rtt4" RelativeTo="Element" Width="300px" AutoCloseDelay="10000"
                                Height="100px" TargetControlID="imgNewRecord" IsClientID="false" Animation="Fade" Position="TopRight">
                                By choosing <b>Allow</b> you're giving this office's analyzers the ability to begin a new record period before the previous period has been approved.
                            </telerik:RadToolTip>
                        </FormTemplate>
                    </EditFormSettings>
                    <EditItemStyle BackColor="#ebe2c0" />
                </MasterTableView>
            </telerik:RadGrid>
        </asp:Panel>
    </div>
</asp:Content>
