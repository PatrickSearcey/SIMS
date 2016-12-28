<%@ Page Title="" Language="C#" MasterPageFile="~/SIMSSingleMenu.Master" AutoEventWireup="true" CodeBehind="ViewDocs.aspx.cs" Inherits="SIMS2017.StationDoc.ViewDocs" %>
<%@ Register Src="~/Control/SitePageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../styles/stationdoc.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <telerik:RadAjaxManager ID="ram" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="rtsMain">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rmp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbCustom">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlElements" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlCustomReport" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbBack">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlElements" />
                    <telerik:AjaxUpdatedControl ControlID="pnlCustomReport" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="ralp" runat="server" Skin="Bootstrap" />

    <uc:PageHeading id="ph1" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph2" runat="server">
    <div class="mainContent">
        <telerik:RadTabStrip RenderMode="Lightweight" runat="server" ID="rtsMain" Orientation="HorizontalTop" MultiPageID="rmp" Skin="Bootstrap">
            <Tabs>
                <telerik:RadTab Text="Station Description"></telerik:RadTab>
                <telerik:RadTab Text="Station Analysis"></telerik:RadTab>
                <telerik:RadTab Text="Manuscript"></telerik:RadTab>
                <telerik:RadTab Text="Custom Report"></telerik:RadTab>
            </Tabs>
        </telerik:RadTabStrip>
        <telerik:RadMultiPage runat="server" ID="rmp" CssClass="multiPage">
            <telerik:RadPageView runat="server" ID="rpvSDESC">
                <h3>Station Description View</h3>
                <div class="LastRevised">
                    <p style="padding:5px 0 0 10px;"><b>Most recent revision:</b> <asp:Literal ID="ltlSDESCRevisedDt" runat="server" /><br />
                    <b>Revised by:</b> <asp:Literal ID="ltlSDESCRevisedBy" runat="server" /></p>
                </div>
                <asp:DataList ID="dlSDESC" runat="server">
                    <ItemTemplate>
                        <b><%# Eval("ElementName") %>.--</b> <%# Eval("ElementInfo") %>
                        <p></p>
                    </ItemTemplate>
                </asp:DataList>
            </telerik:RadPageView>
            <telerik:RadPageView runat="server" ID="rpvSANAL">
                <h3>Station Analysis View</h3>
                <div class="LastRevised">
                    <p style="padding:5px 0 0 10px;"><b>Most recent revision:</b> <asp:Literal ID="ltlSANALRevisedDt" runat="server" /><br />
                    <b>Revised by:</b> <asp:Literal ID="ltlSANALRevisedBy" runat="server" /></p>
                </div>
                <asp:DataList ID="dlSANAL" runat="server">
                    <ItemTemplate>
                        <b><%# Eval("ElementName") %>.--</b> <%# Eval("ElementInfo") %>
                        <p></p>
                    </ItemTemplate>
                </asp:DataList>
            </telerik:RadPageView>
            <telerik:RadPageView runat="server" ID="rpvMANU">
                <h3>Manuscript View</h3>
                <div class="LastRevised">
                    <p style="padding:5px 0 0 10px;"><b>Published Site Name:</b> <asp:Literal ID="ltlPublishedName" runat="server" /><br />
                    <b>Published Basin Name:</b> <asp:Literal ID="ltlPublishedBasin" runat="server" /></p>
                    <p style="padding:5px 0 0 10px;"><b>Most recent revision:</b> <asp:Literal ID="ltlMANURevisedDt" runat="server" /><br />
                    <b>Revised by:</b> <asp:Literal ID="ltlMANURevisedBy" runat="server" /></p>
                </div>
                <asp:DataList ID="dlMANU" runat="server">
                    <ItemTemplate>
                        <b><%# Eval("ElementName") %>.--</b> <%# Eval("ElementInfo") %>
                        <p></p>
                    </ItemTemplate>
                </asp:DataList>
            </telerik:RadPageView>
            <telerik:RadPageView runat="server" ID="rpvCustom">
                <asp:Panel ID="pnlElements" runat="server">
                    <h3>Custom Report View</h3>
                    <p>Please choose the elements to be displayed on the report:</p>
                    <telerik:RadCheckBoxList ID="rcblElements" runat="server" DataBindings-DataTextField="element_nm" DataBindings-DataValueField="element_id" OnItemDataBound="rcblElements_ItemDataBound" Skin="Bootstrap" />
                    <p>Give your report a custom title:
                    <telerik:RadTextBox ID="rtbReportTitle" runat="server" Width="300px" Skin="Bootstrap" /></p>
                    <telerik:RadButton ID="rbCustom" runat="server" Text="Create Report" OnCommand="rbCustom_Command" Skin="Bootstrap" />
                </asp:Panel>
                <asp:Panel ID="pnlCustomReport" runat="server">
                    <h3><asp:Literal ID="ltlReportTitle" runat="server" /></h3>
                    <asp:DataList ID="dlCustom" runat="server">
                        <ItemTemplate>
                            <div class="RevisionHistory">
                                Revised By: <%# Eval("RevisedBy") %> Date Revised: <%# Eval("RevisedDate") %> 
                                (<a href='<%# String.Format("Archive.aspx?element_id={0}&site_id={1}", Eval("ElementID"), Eval("SiteID")) %>' target="_blank">revision history</a>)
                            </div>
                            <b><%# Eval("ElementName") %>.--</b> <%# Eval("ElementInfo") %>
                            <p></p>
                        </ItemTemplate>
                    </asp:DataList>
                    <telerik:RadButton ID="rbBack" runat="server" Text="Go Back" OnCommand="rbBack_Command" Skin="Bootstrap" />
                </asp:Panel>
            </telerik:RadPageView>
        </telerik:RadMultiPage>
    </div>
</asp:Content>
