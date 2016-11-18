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
                    <telerik:AjaxUpdatedControl ControlID="pnlEditSiteSpecificInfo" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>

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
                                    <asp:Panel ID="pnlEditSiteSpecificInfo" runat="server" CssClass="SiteSpecificInfo">
                                        <table style="width:100%">
                                            <tr>
                                                <td width="200">Remote Site?</td>
                                                <td>
                                                    <telerik:RadDropDownList id="rddlRemote" runat="server" AutoPostBack="true" Width="100px" OnSelectedIndexChanged="rddlRemote_SelectedIndexChanged">
                                                        <Items>
                                                            <telerik:DropDownListItem Text="" Value="" />
                                                            <telerik:DropDownListItem Text="Yes" Value="true" />
                                                            <telerik:DropDownListItem Text="No" Value="false" />
                                                        </Items>
                                                    </telerik:RadDropDownList>
                                                    <asp:RequiredFieldValidator ID="rfvRemote" runat="server" ControlToValidate="rddlRemote" ErrorMessage="* required" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Highway or Road Name</td>
                                                <td>
                                                    <telerik:RadTextBox ID="rtbRoadName" runat="server" Width="200px" />
                                                    <asp:RequiredFieldValidator ID="rfvRoadName" runat="server" ControlToValidate="rtbRoadName" ErrorMessage="* required" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Expressway?</td>
                                                <td>
                                                    <telerik:RadDropDownList id="rddlExpressway" runat="server" Width="100px">
                                                        <Items>
                                                            <telerik:DropDownListItem Text="" Value="" />
                                                            <telerik:DropDownListItem Text="Yes" Value="true" />
                                                            <telerik:DropDownListItem Text="No" Value="false" />
                                                        </Items>
                                                    </telerik:RadDropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Bridge Width</td>
                                                <td><telerik:RadNumericTextBox ID="rntbBridgeWidth" runat="server"  Width="100px" /> feet</td>
                                            </tr>
                                            <tr>
                                                <td>Work Zone (Bridge Length)</td>
                                                <td><telerik:RadNumericTextBox ID="rntbWorkZone" runat="server" Width="100px" /> feet</td>
                                            </tr>
                                            <tr>
                                                <td>Lane Width</td>
                                                <td><telerik:RadNumericTextBox ID="rntbLaneWidth" runat="server" Width="100px" /> feet</td>
                                            </tr>
                                            <tr>
                                                <td>Shoulder Width</td>
                                                <td><telerik:RadNumericTextBox ID="rntbShoulderWidth" runat="server" Width="100px" /> feet</td>
                                            </tr>
                                            <tr>
                                                <td>Speed Limit</td>
                                                <td><telerik:RadNumericTextBox ID="rntbSpeedLimit" runat="server" Width="100px" /> mph</td>
                                            </tr>
                                            <tr>
                                                <td>Number of Lanes</td>
                                                <td><telerik:RadNumericTextBox ID="rntbLaneNumber" runat="server" Width="100px" /></td>
                                            </tr>
                                            <tr>
                                                <td>Traffic Flow</td>
                                                <td>
                                                    <telerik:RadDropDownList id="rddlFlow2Way" runat="server" Width="100px">
                                                        <Items>
                                                            <telerik:DropDownListItem Text="" Value="" />
                                                            <telerik:DropDownListItem Text="two-way" Value="true" />
                                                            <telerik:DropDownListItem Text="one-way" Value="false" />
                                                        </Items>
                                                    </telerik:RadDropDownList>
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
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Divided Highway?</td>
                                                <td>
                                                    <telerik:RadDropDownList id="rddlDividedHighway" runat="server" Width="100px">
                                                        <Items>
                                                            <telerik:DropDownListItem Text="" Value="" />
                                                            <telerik:DropDownListItem Text="Yes" Value="true" />
                                                            <telerik:DropDownListItem Text="No" Value="false" />
                                                        </Items>
                                                    </telerik:RadDropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Median?</td>
                                                <td>
                                                    <telerik:RadDropDownList id="rddlMedian" runat="server" Width="100px">
                                                        <Items>
                                                            <telerik:DropDownListItem Text="" Value="" />
                                                            <telerik:DropDownListItem Text="Yes" Value="true" />
                                                            <telerik:DropDownListItem Text="No" Value="false" />
                                                        </Items>
                                                    </telerik:RadDropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Site Specific Notes</td>
                                                <td>
                                                    <telerik:RadTextBox ID="rtbNotes" runat="server" TextMode="MultiLine" Width="400px" />
                                                </td>
                                            </tr>
                                        </table>
                                        <hr />
                                        <telerik:RadButton ID="rbSubmit" runat="server" Text="Submit Changes" OnCommand="rbSubmit_Command" CommandArgument="SiteSpecificInfo" AutoPostBack="true" />
                                        <telerik:RadButton ID="rbCancel" runat="server" Text="Cancel and Reset" OnCommand="rbCancel_Command" CommandArgument="Cancel" AutoPostBack="true" />
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
                                
                                </Content>
                            </telerik:LayoutRow>
                        </Rows>
                    </telerik:CompositeLayoutColumn>
                </Columns>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>
</asp:Content>
