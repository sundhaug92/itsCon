<%@ Page Title="About" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="About.aspx.cs" Inherits="webApp.About" %>

<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">
    <section class="featured">
        <div class="content-wrapper">
            <hgroup class="title">
                <h1>
                    <%: Page.Title %>. </h1>
                <h2>Virtual Learning. Defined </h2>
            </hgroup>
            <p>
                To learn more about this new world of applications, visit <a href="http://github.com/sundhaug92/itscon/"
                    title="itsCon website">http://github.com/sundhaug92/itscon/ </a>. The page features
                <span class="highlight">documentation, forums, and links</span>to help you get the
                most from itsCon. If you have any questions about itsCon <a href="mailto:martinsundhaug@gmail.com"
                    title="send an email to me">send an email to me</a>.
            </p>
        </div>
    </section>
</asp:Content>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <hgroup class="title">
        <h1>
            <%: Page.Title %>.</h1>
        <h2>Xporter webApp</h2>
    </hgroup>
    <article>
        <p>
            Lorem ipsum dolor sit amet, consectetur adipiscing elit. Proin luctus tincidunt
            justo nec tempor. Aliquam erat volutpat. Fusce facilisis ullamcorper consequat.
            Vestibulum non sapien lectus. Nam mi augue, posuere at tempus vel, dignissim vitae
            nulla. Nullam at quam eu sapien mattis ultrices. Quisque quis leo mi, at lobortis
            dolor. Nullam scelerisque facilisis placerat. Fusce a augue erat, malesuada euismod
            dui. Duis iaculis risus id felis volutpat elementum. Fusce blandit iaculis quam
            a cursus. Cras varius tincidunt cursus. Morbi justo eros, adipiscing ac placerat
            sed, posuere et mi. Suspendisse vulputate viverra aliquet. Duis non enim a nibh
            consequat mollis ac tempor lorem. Phasellus elit leo, semper eu luctus et, suscipit
            at lacus. In hac habitasse platea dictumst. Duis dignissim justo sit amet nulla
            pulvinar sodales.
        </p>
        <p>
            Lorem ipsum dolor sit amet, consectetur adipiscing elit. Proin luctus tincidunt
            justo nec tempor. Aliquam erat volutpat. Fusce facilisis ullamcorper consequat.
            Vestibulum non sapien lectus. Nam mi augue, posuere at tempus vel, dignissim vitae
            nulla. Nullam at quam eu sapien mattis ultrices. Quisque quis leo mi, at lobortis
            dolor. Nullam scelerisque facilisis placerat. Fusce a augue erat, malesuada euismod
            dui. Duis iaculis risus id felis volutpat elementum. Fusce blandit iaculis quam
            a cursus. Cras varius tincidunt cursus. Morbi justo eros, adipiscing ac placerat
            sed, posuere et mi. Suspendisse vulputate viverra aliquet. Duis non enim a nibh
            consequat mollis ac tempor lorem. Phasellus elit leo, semper eu luctus et, suscipit
            at lacus. In hac habitasse platea dictumst. Duis dignissim justo sit amet nulla
            pulvinar sodales.
        </p>
        <p>
            Lorem ipsum dolor sit amet, consectetur adipiscing elit. Proin luctus tincidunt
            justo nec tempor. Aliquam erat volutpat. Fusce facilisis ullamcorper consequat.
            Vestibulum non sapien lectus. Nam mi augue, posuere at tempus vel, dignissim vitae
            nulla. Nullam at quam eu sapien mattis ultrices. Quisque quis leo mi, at lobortis
            dolor. Nullam scelerisque facilisis placerat. Fusce a augue erat, malesuada euismod
            dui. Duis iaculis risus id felis volutpat elementum. Fusce blandit iaculis quam
            a cursus. Cras varius tincidunt cursus. Morbi justo eros, adipiscing ac placerat
            sed, posuere et mi. Suspendisse vulputate viverra aliquet. Duis non enim a nibh
            consequat mollis ac tempor lorem. Phasellus elit leo, semper eu luctus et, suscipit
            at lacus. In hac habitasse platea dictumst. Duis dignissim justo sit amet nulla
            pulvinar sodales.
        </p>
    </article>
    <aside>
        <h3>Aside Title</h3>
        <p>
            Fusce facilisis ullamcorper consequat. Vestibulum non sapien lectus. Nam mi augue,
            posuere at tempus vel, dignissim vitae nulla.
        </p>
        <ul>
            <li><a runat="server" href="~/">Home</a></li>
            <li><a runat="server" href="~/About.aspx">About</a></li>
        </ul>
    </aside>
</asp:Content>