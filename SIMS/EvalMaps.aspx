<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/SIMSSiteTopLevel.Master" CodeBehind="EvalMaps.aspx.vb" Inherits="SIMS.EvalMaps" %>
<%@ Register Assembly="GMaps" Namespace="Subgurim.Controles" TagPrefix="cc1" %>
<%@ MasterType virtualPath="~/SIMSSiteTopLevel.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" Runat="Server">
<p class="SITitleFont">National Evaluation Maps</p>
<p>This map provides configuration information about sites associated with 
SIMS.  The results box provides an explanation of the evaluation option. If the option is showing an inconsistency 
between systems, instructions about how to resolve issues are provided. The map can only display 350 sites or less at 
a time. If more than 350 sites are returned, results can be narrowed down by geographic area and by office. The 
maps are updated once every night and reflect current configuration settings.</p>
<asp:UpdatePanel ID="upMain" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel id="pnlMap_Canvas" runat="server" CssClass="Map_Canvas">
            <asp:Panel ID="pnlTopFilterControls" runat="server" CssClass="TopFilterControls">
                <div style="padding:5px;">
                    <div class="SITitleFontSmall" style="margin-bottom:0px;margin-top:0px;">
                    Choose evaluation option:
                    <asp:DropDownList ID="ddlEvalOptions" runat="server" CssClass="SIBodyFont">
                        <asp:ListItem Text="General: Real-time site not registered in SIMS" Value="1" />
                        <asp:ListItem Text="General: Real-time site not active in NWIS" Value="2" />
                        <asp:ListItem Text="Telemetry: Indicated for site no longer real-time" value="3" />
                        <asp:ListItem Text="Telemetry: Not indicated for real-time site" Value="4" />
                        <asp:ListItem Text="Telemetry: Distribution of 100 baud IDs" Value="5" />
                        <asp:ListItem Text="Safety: No information present" Value="6" />
                        <asp:ListItem Text="Safety: SHA present, no TCP for real-time site" Value="7" />
                        <asp:ListItem Text="Safety: TCP present, no SHA for real-time site" Value="8" />
                        <asp:ListItem Text="Safety: Missing emergency information" Value="9" />
                        <asp:ListItem Text="Safety: Duplicate information" Value="10" />
                    </asp:DropDownList>
                    & USGS area:
                    <asp:DropDownList ID="ddlWSC" runat="server" OnSelectedIndexChanged="WSC_Selected"
                        AutoPostBack="True" DataTextField="wsc_nm" DataValueField="wsc_cd"
                        CssClass="SIBodyFont" />
                    <asp:Button ID="btnTopFilters" runat="server" Text="Go!" CssClass="SIBodyFont" 
                        OnCommand="TopFiltersUsed" CommandName="btnTopFilters" />
                    </div>
                </div>
            </asp:Panel>
            <asp:Panel ID="pnlSideFilterControls" runat="server" CssClass="SideFilterControls">
                <div style="padding:5px;">
                    <asp:Panel ID="pnlNotice" runat="server" Visible="false">
                        <p class="SITitleFontSmall" style="margin-top:0px;">Notice:</p>
                        <asp:Panel ID="pnlNoticeIP" runat="server" Width="255" BackColor="#FFFFFF">
                            <div style="padding-left:3px;line-height:18px;padding-top:5px;padding-bottom:5px;">
                            <asp:Label ID="lblNotice" runat="server" Font-Bold="true" ForeColor="Red" />
                            </div>
                        </asp:Panel>
                        <br />
                    </asp:Panel>
                    
                    <asp:Panel ID="pnlFilters" runat="server" Visible="false">
                        <p class="SITitleFontSmall" style="margin-top:0px;">Narrow down the display range:</p>
                        <asp:Panel ID="pnlDisplayOptions" Width="255" runat="server" BackColor="#FFFFFF">
                            <div style="padding-left:3px;line-height:18px;padding-top:5px;padding-bottom:5px;">
                            <asp:Label ID="lblOffice" runat="server" Text="By office:<br />" 
                                Font-Bold="true" />
                            <asp:DropDownList ID="ddlOffice" runat="server" OnSelectedIndexChanged="Office_Selected" 
                                AutoPostBack="True" DataTextField="office_nm" DataValueField="office_cd"
                                CssClass="SIBodyFontSmall" />

                            <asp:Label ID="lblFieldTrip" runat="server" Text="<br /><br />By field trip:<br />" 
                                Font-Bold="true" Visible="false" />
                            <asp:DropDownList ID="ddlFieldTrip" runat="server" CssClass="SIBodyFontSmall"
                                AutoPostBack="True" Visible="false" />
                            </div>
                        </asp:Panel>
                    </asp:Panel>
                    
                    <asp:Panel ID="pnlResults" runat="server">
                        <br />
                        <p class="SITitleFontSmall" style="margin-top:0px;">Results:</p>
                        <asp:Panel ID="pnlResultsIP" runat="server" Width="255" BackColor="#FFFFFF">
                            <div style="padding-left:3px;line-height:18px;padding-top:5px;padding-bottom:5px;">
                            <asp:Label ID="lblResultsSum" runat="server" cssclass="SIBodyFontSmall" />
                            <asp:Label ID="lblResultsCount" runat="server" cssclass="SIBodyFontSmall" Font-Bold="true" />
                            <asp:Hyperlink ID="hlResultsList" runat="server" CssClass="SIBodyFontSmall" 
                                Font-Italic="true" Target="_blank" />
                            <asp:Label ID="lblWarning" runat="server" CssClass="SIBodyFontSmall"
                                Font-Bold="true" Font-Italic="true" ForeColor="Orange" />
                            </div>
                        </asp:Panel>
                    </asp:Panel>
                    
                    <asp:Panel ID="pnlLegend" runat="server">
                        <br />
                        <p class="SITitleFontSmall" style="margin-top:0px;">Legend:</p>
                        <asp:Panel ID="pnlLegendIP" runat="server" Width="255" BackColor="#FFFFFF">
                            <div style="padding-left:3px;padding-top:5px;padding-bottom:10px;">
                            <table width="100%" cellpadding="0" cellspacing="0" border="0">
                                <tr>
                                    <td><img src="images/SiteIcons/SW.png" alt="Surface-Water Sites" align="middle" /> SW Sites</td>
                                    <td><img src="images/SiteIcons/GW.png" alt="Groundwater Sites" align="middle" /> GW Sites</td>
                                </tr>
                                <tr>
                                    <td><img src="images/SiteIcons/SP.png" alt="Spring Sites" align="middle" /> Spring Sites</td>
                                    <td><img src="images/SiteIcons/CL.png" alt="Atmospheric Sites" align="middle" /> CLIM Sites</td>
                                </tr>
                                <tr>
                                    <td colspan="2"><img src="images/SiteIcons/OT.png" alt="Other Site Types" align="middle" /> Other Site Types</td>
                                </tr>
                            </table>
                            </div>
                        </asp:Panel>
                    </asp:Panel>
                </div>
            </asp:Panel>
            <asp:Panel id="pnlMap" runat="server" CssClass="Map">
                <cc1:gmap id="GMap" runat="server" Width="100%" Height="100%" 
                    enableServerEvents="False" OnMarkerClick="Marker_Clicked"></cc1:gmap>
            </asp:Panel>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
<p>References to non-U.S. Department of the Interior (DOI) products do not constitute an endorsement 
    by the DOI. By viewing the Google Maps API on this web site the user agrees to these 
    <a href="http://code.google.com/apis/maps/terms.html">TERMS</a> of Service set forth by Google. 
</p>
</asp:Content>
