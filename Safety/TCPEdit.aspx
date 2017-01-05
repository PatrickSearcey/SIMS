<%@ Page Title="" Language="C#" MasterPageFile="~/SafetySingleMenu.Master" AutoEventWireup="true" CodeBehind="TCPEdit.aspx.cs" Inherits="Safety.TCPEdit" %>
<%@ Register Src="~/Control/SitePageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="styles/reports.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <telerik:RadAjaxManager ID="ram" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="rddlRemote">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlEditSiteSpecificInfo" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbSubmit">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlEditSiteSpecificInfo" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlTCPs" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbCancel">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlEditSiteSpecificInfo" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="lbAddPlanVI">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlTCPs" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="dlTCPs">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="dlTCPs" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rddlFlow2Way">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rddlFlaggers" />
                    <telerik:AjaxUpdatedControl ControlID="rfvFlaggers" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rntbLaneNumber">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rddlFlaggers" />
                    <telerik:AjaxUpdatedControl ControlID="rfvFlaggers" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rntbShoulderWidth">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rddlFlaggers" />
                    <telerik:AjaxUpdatedControl ControlID="rfvFlaggers" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="ralp" runat="server" Skin="Bootstrap" />

    <uc:PageHeading id="ph1" runat="server" />

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph2" runat="server">
    <telerik:RadPageLayout ID="rpl1" runat="server" GridType="Fluid" CssClass="mainContent">
        <Rows>
            <telerik:LayoutRow>
                <Columns>
                    <telerik:CompositeLayoutColumn Span="12" SpanMd="12" SpanSm="12" HiddenXs="true">
                        <Rows>
                            <telerik:LayoutRow>
                                <Content>
                                    <h4>Site Specific Information</h4>
                                    <asp:Panel ID="pnlEditSiteSpecificInfo" runat="server" CssClass="TCPInfo">
                                        <table style="width:600px">
                                            <tr>
                                                <td colspan="2" style="font-weight:bold;padding-bottom:10px;">
                                                    <asp:Literal ID="ltlLastUpdated" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width="200">Remote Site?</td>
                                                <td>
                                                    <telerik:RadDropDownList id="rddlRemote" runat="server" AutoPostBack="true" Width="100px" OnSelectedIndexChanged="rddlRemote_SelectedIndexChanged">
                                                        <Items>
                                                            <telerik:DropDownListItem Text="" Value="" />
                                                            <telerik:DropDownListItem Text="Yes" Value="True" />
                                                            <telerik:DropDownListItem Text="No" Value="False" />
                                                        </Items>
                                                    </telerik:RadDropDownList>
                                                    <asp:RequiredFieldValidator ID="rfvRemote" runat="server" ControlToValidate="rddlRemote" ErrorMessage="* required" ForeColor="Red" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Highway or Road Name</td>
                                                <td>
                                                    <telerik:RadTextBox ID="rtbRoadName" runat="server" Width="200px" />
                                                    <asp:RequiredFieldValidator ID="rfvRoadName" runat="server" ControlToValidate="rtbRoadName" ErrorMessage="* required" ForeColor="Red" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Expressway?</td>
                                                <td>
                                                    <telerik:RadDropDownList id="rddlExpressway" runat="server" Width="100px">
                                                        <Items>
                                                            <telerik:DropDownListItem Text="" Value="" />
                                                            <telerik:DropDownListItem Text="Yes" Value="True" />
                                                            <telerik:DropDownListItem Text="No" Value="False" />
                                                        </Items>
                                                    </telerik:RadDropDownList>
                                                    <asp:RequiredFieldValidator ID="rfvExpressway" runat="server" ControlToValidate="rddlExpressway" ErrorMessage="* required" ForeColor="Red" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Bridge Width</td>
                                                <td>
                                                    <telerik:RadNumericTextBox ID="rntbBridgeWidth" runat="server"  Width="100px" NumberFormat-DecimalDigits="0" /> feet
                                                    <asp:RequiredFieldValidator ID="rfvBridgeWidth" runat="server" ControlToValidate="rntbBridgeWidth" ErrorMessage="* required" ForeColor="Red" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Work Zone (Bridge Length)</td>
                                                <td>
                                                    <telerik:RadNumericTextBox ID="rntbWorkZone" runat="server" Width="100px" NumberFormat-DecimalDigits="0" /> feet
                                                    <asp:RequiredFieldValidator ID="rfvWorkZone" runat="server" ControlToValidate="rntbWorkZone" ErrorMessage="* required" ForeColor="Red" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Lane Width</td>
                                                <td>
                                                    <telerik:RadNumericTextBox ID="rntbLaneWidth" runat="server" Width="100px" NumberFormat-DecimalDigits="0" /> feet
                                                    <asp:RequiredFieldValidator ID="rfvLaneWidth" runat="server" ControlToValidate="rntbLaneWidth" ErrorMessage="* required" ForeColor="Red" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Shoulder Width</td>
                                                <td>
                                                    <telerik:RadNumericTextBox ID="rntbShoulderWidth" runat="server" Width="100px" OnTextChanged="RunPlanLogic" AutoPostBack="true" NumberFormat-DecimalDigits="0" /> feet
                                                    <asp:RequiredFieldValidator ID="rfvShoulderWidth" runat="server" ControlToValidate="rntbShoulderWidth" ErrorMessage="* required" ForeColor="Red" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Speed Limit</td>
                                                <td>
                                                    <telerik:RadNumericTextBox ID="rntbSpeedLimit" runat="server" Width="100px" NumberFormat-DecimalDigits="0" /> mph
                                                    <asp:RequiredFieldValidator ID="rfvSpeedLimit" runat="server" ControlToValidate="rntbSpeedLimit" ErrorMessage="* required" ForeColor="Red" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Number of Lanes</td>
                                                <td>
                                                    <telerik:RadNumericTextBox ID="rntbLaneNumber" runat="server" Width="100px" OnTextChanged="RunPlanLogic" AutoPostBack="true" NumberFormat-DecimalDigits="0" />
                                                    <asp:RequiredFieldValidator ID="rfvLaneNumber" runat="server" ControlToValidate="rntbLaneNumber" ErrorMessage="* required" ForeColor="Red" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Traffic Flow</td>
                                                <td>
                                                    <telerik:RadDropDownList id="rddlFlow2Way" runat="server" Width="100px" OnSelectedIndexChanged="RunPlanLogic" AutoPostBack="true">
                                                        <Items>
                                                            <telerik:DropDownListItem Text="" Value="" />
                                                            <telerik:DropDownListItem Text="two-way" Value="True" />
                                                            <telerik:DropDownListItem Text="one-way" Value="False" />
                                                        </Items>
                                                    </telerik:RadDropDownList>
                                                    <asp:RequiredFieldValidator ID="rfvFlow2Way" runat="server" ControlToValidate="rddlFlow2Way" ErrorMessage="* required" ForeColor="Red" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Traffic Volume</td>
                                                <td>
                                                    <telerik:RadDropDownList id="rddlTrafficVolume" runat="server" Width="100px">
                                                        <Items>
                                                            <telerik:DropDownListItem Text="" Value="" />
                                                            <telerik:DropDownListItem Text="low" Value="low" />
                                                            <telerik:DropDownListItem Text="moderate" Value="moderate" />
                                                            <telerik:DropDownListItem Text="high" Value="high" />
                                                        </Items>
                                                    </telerik:RadDropDownList>
                                                    <asp:RequiredFieldValidator ID="rfvTrafficVolume" runat="server" ControlToValidate="rddlTrafficVolume" ErrorMessage="* required" ForeColor="Red" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Divided Highway?</td>
                                                <td>
                                                    <telerik:RadDropDownList id="rddlDividedHighway" runat="server" Width="100px">
                                                        <Items>
                                                            <telerik:DropDownListItem Text="" Value="" />
                                                            <telerik:DropDownListItem Text="Yes" Value="True" />
                                                            <telerik:DropDownListItem Text="No" Value="False" />
                                                        </Items>
                                                    </telerik:RadDropDownList>
                                                    <asp:RequiredFieldValidator ID="rfvDividedHighway" runat="server" ControlToValidate="rddlDividedHighway" ErrorMessage="* required" ForeColor="Red" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Median?</td>
                                                <td>
                                                    <telerik:RadDropDownList id="rddlMedian" runat="server" Width="100px">
                                                        <Items>
                                                            <telerik:DropDownListItem Text="" Value="" />
                                                            <telerik:DropDownListItem Text="Yes" Value="True" />
                                                            <telerik:DropDownListItem Text="No" Value="False" />
                                                        </Items>
                                                    </telerik:RadDropDownList>
                                                    <asp:RequiredFieldValidator ID="rfvMedian" runat="server" ControlToValidate="rddlMedian" ErrorMessage="* required" ForeColor="Red" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Flaggers Required?</td>
                                                <td>
                                                    <telerik:RadDropDownList id="rddlFlaggers" runat="server" Width="100px">
                                                        <Items>
                                                            <telerik:DropDownListItem Text="" Value="" />
                                                            <telerik:DropDownListItem Text="Yes" Value="True" />
                                                            <telerik:DropDownListItem Text="No" Value="False" />
                                                        </Items>
                                                    </telerik:RadDropDownList>
                                                    <asp:RequiredFieldValidator ID="rfvFlaggers" runat="server" ControlToValidate="rddlFlaggers" ErrorMessage="* required" ForeColor="Red" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Site Specific Notes</td>
                                                <td>
                                                    <telerik:RadTextBox ID="rtbNotes" runat="server" TextMode="MultiLine" Width="400px" Height="100px" />
                                                </td>
                                            </tr>
                                        </table>
                                        <hr />
                                        <asp:Literal ID="ltlNote" runat="server" /><br />
                                        <div style="padding-top:5px;">
                                            <telerik:RadButton ID="rbSubmit" runat="server" Text="Submit Changes" OnCommand="rbSubmit_Command" CommandArgument="SiteSpecificInfo" AutoPostBack="true" />
                                            <telerik:RadButton ID="rbCancel" runat="server" Text="Cancel and Reset" OnCommand="rbCancel_Command" CommandArgument="Cancel" AutoPostBack="true" CausesValidation="false" />
                                        </div>
                                    </asp:Panel>
                                </Content>
                            </telerik:LayoutRow>
                        </Rows>
                    </telerik:CompositeLayoutColumn>
                </Columns>
            </telerik:LayoutRow>
            <telerik:LayoutRow>
                <Columns>
                    <telerik:CompositeLayoutColumn  Span="12" SpanMd="12" SpanSm="12" HiddenXs="true">
                        <Rows>
                            <telerik:LayoutRow>
                                <Content>
                                    <h4>Traffic Control Plans</h4>
                                    <asp:Panel ID="pnlTCPs" runat="server" CssClass="TCPInfo">
                                        <asp:DataList ID="dlTCPs" runat="server" DataKeyField="TCPID" 
                                            OnEditCommand="dlTCPs_EditCommand" 
                                            OnCancelCommand="dlTCPs_CancelCommand"
                                            OnUpdateCommand="dlTCPs_UpdateCommand" 
                                            OnItemCreated="dlTCPs_ItemCreated" 
                                            OnDeleteCommand="dlTCPs_DeleteCommand">
                                            <ItemTemplate>
                                                <a href='<%# Eval("TCPLink") %>'><%# Eval("TCPName") %></a><br />
                                                <div class="PlanInfo">
                                                    <b>Work Area Activity:</b><br />
                                                    <%# Eval("WorkAreaActivity") %><br />
                                                    <b>Plan Specific Notes:</b><br />
                                                    <%# Eval("PlanRemarks") %><br /><asp:LinkButton ID="lbPlanRemarks" runat="server" Text="edit plan info" CommandName="edit" Font-Bold="true" /> |
                                                    <asp:LinkButton ID="lbDelete" runat="server" Text="delete plan" Font-Bold="true" CommandName="delete" />
                                                </div>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <a href='<%# Eval("TCPLink") %>'><%# Eval("TCPName") %></a><br />
                                                <div class="PlanInfo">
                                                    <b>Work Area Activity:</b>
                                                    <telerik:RadTextBox id="rtbWAA" runat="server" Text='<%# Eval("WorkAreaActivity") %>' Font-Size="Small" /><br />
                                                    <b>Plan Specific Notes:</b><br />
                                                    <telerik:RadTextBox id="rtbPlanRemarks" runat="server" TextMode="MultiLine" Text='<%# Eval("PlanRemarks") %>' Font-Size="Small" Width="200px" /><br /><asp:LinkButton ID="lbUpdateRemarks" runat="server" Text="save" CommandName="update" Font-Bold="true" /> |
                                                    <asp:LinkButton ID="lbCancel" runat="server" Text="cancel" CommandName="cancel" Font-Bold="true" />
                                                </div>
                                            </EditItemTemplate>
                                        </asp:DataList>
                                        <br />
                                        <asp:Image ID="imgBullet" runat="server" ImageUrl="~/images/bullet.png" AlternateText="bullet" /> <asp:LinkButton ID="lbAddPlanVI" runat="server" Text="Click here to add TCP VI - Plan Too Complicated" OnCommand="lbAddPlanVI_Command" CommandArgument="AddVI" />
                                    </asp:Panel>
                                </Content>
                            </telerik:LayoutRow>
                        </Rows>
                    </telerik:CompositeLayoutColumn>
                </Columns>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>
</asp:Content>
