<%@ Page Title="" Language="C#" MasterPageFile="~/SIMSSingleMenu.Master" AutoEventWireup="true" CodeBehind="StationInfo.aspx.cs" Inherits="SIMS2017.StationInfo" %>
<%@ Register Src="~/Control/SitePageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="styles/stationinfo.css" rel="stylesheet" />
    <script type="text/javascript">
        function openWin(_id) {
            var oWnd = radopen("Modal/FieldTripEdit.aspx?site_id=" + _id, "rwEditFieldTrips");
        }

        function OnClientClose(oWnd, args) {
            //get the transferred arguments
            var arg = args.get_argument();
            if (arg) {
                var fieldTrips = arg.fieldTrips;
                var hfFieldTripIDs = document.getElementById("<%= hfFieldTripIDs.ClientID %>");
                hfFieldTripIDs.value = fieldTrips;
                $find("<%= ram.ClientID %>").ajaxRequest("Rebind");
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <telerik:RadAjaxManager ID="ram" runat="server" OnAjaxRequest="ram_AjaxRequest">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="lbEditPubName">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlPubNameView" />
                    <telerik:AjaxUpdatedControl ControlID="pnlPubNameEdit" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbPubName">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlPubNameView" />
                    <telerik:AjaxUpdatedControl ControlID="pnlPubNameEdit" />
                    <telerik:AjaxUpdatedControl ControlID="ph1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="lbEditOffice">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlOfficeView" />
                    <telerik:AjaxUpdatedControl ControlID="pnlOfficeEdit" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rddlOffice">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlOfficeView" />
                    <telerik:AjaxUpdatedControl ControlID="pnlOfficeEdit" />
                    <telerik:AjaxUpdatedControl ControlID="ph1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="ram">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlFieldTripView" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadWindowManager RenderMode="Lightweight" ID="rwm" ShowContentDuringLoad="false" VisibleStatusbar="false" ReloadOnShow="true" runat="server" EnableShadow="true">
        <Windows>
            <telerik:RadWindow RenderMode="Lightweight" ID="rwEditFieldTrips" runat="server" Behaviors="Close" OnClientClose="OnClientClose" Width="900" Height="400" />
        </Windows>
    </telerik:RadWindowManager>
    <asp:HiddenField ID="hfFieldTripIDs" runat="server" />

    <uc:PageHeading id="ph1" runat="server" />
    <div class="linkbar">
        <div style="float:left;">
            <telerik:RadTextBox ID="rtbSiteNo" runat="server" EmptyMessage="enter site number" />
            <telerik:RadButton ID="rbJump" runat="server" Text="jump!" OnClick="rbJump_Click" />
        </div>
        <div style="float:right;">
            <asp:HyperLink ID="hlNWISWeb" runat="server" Text="Go to NWISWeb" Target="_blank" /> |
            <asp:HyperLink ID="hlNWISOpsRequest" runat="server" Text="NWIS Ops Request" /> |
            <a href="mailto:GS-W_Help_SIMS@usgs.gov">GS-W Help SIMS</a>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph2" runat="server">
    <telerik:RadPageLayout ID="rpl1" runat="server" GridType="Fluid" CssClass="mainContent">
        <Rows>
            <telerik:LayoutRow>
                <Columns>
                    <telerik:CompositeLayoutColumn Span="6" SpanMd="6" SpanSm="6" HiddenXs="true">
                        <Rows>
                            <telerik:LayoutRow>
                                <Content>
                                    <h4>Station Details</h4>
                                    <asp:Panel ID="pnlPubNameView" runat="server">
                                        Published name: <b><asp:Literal ID="ltlPubName" runat="server" /></b> <asp:LinkButton ID="lbEditPubName" runat="server" Text="edit" OnCommand="Edit_Command" CommandArgument="PubName" />
                                    </asp:Panel>
                                    <asp:Panel ID="pnlPubNameEdit" runat="server" Visible="false">
                                        Published name: <telerik:RadTextBox ID="rtbPubName" runat="server" Width="300px" /> <telerik:RadButton ID="rbPubName" runat="server" Text="commit" OnCommand="Commit_Command" CommandArgument="PubName" />
                                    </asp:Panel>
                                    <asp:Panel ID="pnlOfficeView" runat="server">
                                        Office assignment: <b><asp:Literal ID="ltlOffice" runat="server" /></b> <asp:LinkButton ID="lbEditOffice" runat="server" Text="edit" OnCommand="Edit_Command" CommandArgument="Office" />
                                    </asp:Panel>
                                    <asp:Panel ID="pnlOfficeEdit" runat="server" Visible="false">
                                        Office assignment: <telerik:RadDropDownList ID="rddlOffice" runat="server" DataValueField="office_id" DataTextField="office_nm" OnSelectedIndexChanged="Commit_Command" AutoPostBack="true" Width="350px" />
                                    </asp:Panel>
                                    <asp:Panel ID="pnlFieldTripView" runat="server">
                                        Field trip(s): <b><asp:Literal ID="ltlFieldTrip" runat="server" /></b> <asp:LinkButton ID="lbEditFieldTrip" runat="server" Text="edit" />
                                    </asp:Panel>
                                </Content>
                            </telerik:LayoutRow>
                            <telerik:LayoutRow>
                                <Content>
                                    <h4>Safety</h4>
                                </Content>
                            </telerik:LayoutRow>
                        </Rows>
                    </telerik:CompositeLayoutColumn>
                    <telerik:CompositeLayoutColumn Span="6" SpanMd="6" SpanSm="6" HiddenXs="true">
                        <Rows>
                            <telerik:LayoutRow>
                                <Content>
                                    <h4>Station Documents</h4>
                                </Content>
                            </telerik:LayoutRow>
                            <telerik:LayoutRow>
                                <Content>
                                    <h4>Continuous Records Processing</h4>
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
                                <h4>DCP/Realtime Ops</h4>
                                </Content>
                            </telerik:LayoutRow>
                        </Rows>
                    </telerik:CompositeLayoutColumn>
                </Columns>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>
</asp:Content>
