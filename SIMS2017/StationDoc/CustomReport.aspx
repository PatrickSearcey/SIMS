<%@ Page Title="" Language="C#" MasterPageFile="~/SIMSSingleMenu.Master" AutoEventWireup="true" CodeBehind="CustomReport.aspx.cs" Inherits="SIMS2017.StationDoc.CustomReport" %>
<%@ Register Src="~/Control/SitePageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../styles/stationdoc.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <telerik:RadAjaxManager ID="ram" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="rrblWaterBodyType">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlElements" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlCustomReport" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="rrblOffice" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rrblOffice">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlElements" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlCustomReport" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="rrblWaterBodyType" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbCustom">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlElements" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlCustomReport" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="rtsMain" />
                    <telerik:AjaxUpdatedControl ControlID="rmp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbBack">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlElements" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlCustomReport" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="rtsMain" />
                    <telerik:AjaxUpdatedControl ControlID="rmp" />
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
                <telerik:RadTab Text="Report By Water Body Type"></telerik:RadTab>
                <telerik:RadTab Text="Report By Office"></telerik:RadTab>
            </Tabs>
        </telerik:RadTabStrip>
        <telerik:RadMultiPage runat="server" ID="rmp" CssClass="multiPage">
            <telerik:RadPageView runat="server" ID="rpvWaterBody">
                <h4>Please select the appropriate option to update the possible elements for selection:</h4>
                <telerik:RadRadioButtonList ID="rrblWaterBodyType" runat="server" Skin="Bootstrap" OnSelectedIndexChanged="UpdateElementList" AutoPostBack="true" Layout="Flow">
                    <Items>
                        <telerik:ButtonListItem Text="Spring" Value="5" />
                        <telerik:ButtonListItem Text="Estuary" Value="3" />
                        <telerik:ButtonListItem Text="Lake" Value="2" />
                        <telerik:ButtonListItem Text="Stream" Value="1" />
                        <telerik:ButtonListItem Text="Groundwater" Value="6" />
                    </Items>
                </telerik:RadRadioButtonList>
            </telerik:RadPageView>
            <telerik:RadPageView runat="server" ID="rpvOffice">
                <h4>Please select the appropriate option to update the possible elements for selection</h4>
                <telerik:RadRadioButtonList ID="rrblOffice" runat="server" Skin="Bootstrap" DataBindings-DataValueField="office_id" DataBindings-DataTextField="office_nm"
                    OnSelectedIndexChanged="UpdateElementList" AutoPostBack="true" />
            </telerik:RadPageView>
        </telerik:RadMultiPage>

        <asp:Panel ID="pnlElements" runat="server">
            <h3>Please choose the elements to be displayed on the report:</h3>
            <h5>Station Description Elements</h5>
            <telerik:RadCheckBoxList ID="rcblSDESCElements" runat="server" DataBindings-DataTextField="element_nm" DataBindings-DataValueField="element_id" Skin="Bootstrap" CssClass="SDESC" />
            <h5>Station Analysis Elements</h5>
            <telerik:RadCheckBoxList ID="rcblSANALElements" runat="server" DataBindings-DataTextField="element_nm" DataBindings-DataValueField="element_id" Skin="Bootstrap" CssClass="SANAL" />
            <h5>Manuscript Elements</h5>
            <telerik:RadCheckBoxList ID="rcblMANUElements" runat="server" DataBindings-DataTextField="element_nm" DataBindings-DataValueField="element_id" Skin="Bootstrap" CssClass="MANU" />
            <p>Give your report a custom title:
            <telerik:RadTextBox ID="rtbReportTitle" runat="server" Width="300px" Skin="Bootstrap" /></p>
            <telerik:RadButton ID="rbCustom" runat="server" Text="Create Report" OnCommand="rbCustom_Command" Skin="Bootstrap" />
        </asp:Panel>

        <asp:Panel ID="pnlCustomReport" runat="server">
            <h3><asp:Literal ID="ltlReportTitle" runat="server" /></h3>
            <asp:ListView ID="lvCustom" runat="server" OnItemDataBound="lvCustom_ItemDataBound">
                <ItemTemplate>
                    <b>Station No.: <a href='<%# String.Format("../StationInfo.aspx?site_id={0}", Eval("SiteID")) %>' target="_blank"><%# Eval("SiteNo") %> <%# Eval("StationName") %></a> &nbsp;&nbsp;&nbsp;
                        Agency: <%# Eval("AgencyCd") %>
                    </b>
                    <asp:DataList ID="dlCustomElements" runat="server">
                        <ItemTemplate>
                            <div class="RevisionHistory">
                                Revised By: <%# Eval("RevisedBy") %> Date Revised: <%# Eval("RevisedDate") %> 
                                (<a href='<%# String.Format("Archive.aspx?element_id={0}&site_id={1}", Eval("ElementID"), Eval("SiteID")) %>' target="_blank">revision history</a>)
                            </div>
                            <b><%# Eval("ElementName") %>.--</b> <%# Eval("ElementInfo") %>
                            <p></p>
                        </ItemTemplate>
                    </asp:DataList>
                    <hr />
                </ItemTemplate>
            </asp:ListView>
            <telerik:RadButton ID="rbBack" runat="server" Text="Go Back" OnCommand="rbBack_Command" Skin="Bootstrap" />
        </asp:Panel>
    </div>
</asp:Content>
