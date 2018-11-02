<template>
    <ul :id="barId" class="sidenav sidenav-fixed portal-slidebar-nav">
        <li>
            <div class="user-view">
                <div class="background">
                    <img class="responsive-img" src="images/office.jpg">
                </div>
                <a href="#user">
                    <img class="circle" src="images/company.jpg">
                </a>
                <a href="#name">
                    <span class="white-text name">{companyName}}</span>
                </a>
                <a href="#email">
                    <span class="white-text email">{companyMail}}</span>
                </a>
            </div>
        </li>

        <li class="no-padding">
            <ul class="collapsible collapsible-accordion">
                <li>
                    <a class="collapsible-header">{goup1.Title}}<i class="material-icons chevron">chevron_left</i></a>
                    <div class="collapsible-body">
                        <ul>
                            <li>
                                <a href="#" class="waves-effect">{query11.Title}}<i class="material-icons">landscape</i></a>
                            </li>
                            <li>
                                <a href="#" class="waves-effect">{query12.Title}}<i class="material-icons">landscape</i></a>
                            </li>
                            <li>
                                <a href="#" class="waves-effect">{query13.Title}}<i class="material-icons">landscape</i></a>
                            </li>
                            <li>
                                <a href="#" class="waves-effect">{query14.Title}}<i class="material-icons">landscape</i></a>
                            </li>
                        </ul>
                    </div>
                </li>
                <li>
                    <a class="collapsible-header">{goup2.Title}}<i class="material-icons chevron">chevron_left</i></a>
                    <div class="collapsible-body">
                        <ul>
                            <li>
                                <a href="#" class="waves-effect">{query21.Title}}<i class="material-icons">landscape</i></a>
                            </li>
                            <li>
                                <a href="#" class="waves-effect">{query22.Title}}<i class="material-icons">landscape</i></a>
                            </li>
                            <li>
                                <a href="#" class="waves-effect">{query23.Title}}<i class="material-icons">landscape</i></a>
                            </li>
                            <li>
                                <a href="#" class="waves-effect">{query24.Title}}<i class="material-icons">landscape</i></a>
                            </li>
                        </ul>
                    </div>
                </li>
                <li>
                    <a class="collapsible-header">{goup3.Title}}<i class="material-icons chevron">chevron_left</i></a>
                    <div class="collapsible-body">
                        <ul>
                            <li>
                                <a href="#" class="waves-effect">{query31.Title}}<i class="material-icons">landscape</i></a>
                            </li>
                            <li>
                                <a href="#" class="waves-effect">{query32.Title}}<i class="material-icons">landscape</i></a>
                            </li>
                            <li>
                                <a href="#" class="waves-effect">{query33.Title}}<i class="material-icons">landscape</i></a>
                            </li>
                            <li>
                                <a href="#" class="waves-effect">{query34.Title}}<i class="material-icons">landscape</i></a>
                            </li>
                        </ul>
                    </div>
                </li>
                <li>
                    <a class="collapsible-header">{goup4.Title}}<i class="material-icons chevron">chevron_left</i></a>
                    <div class="collapsible-body">
                        <ul>
                            <li>
                                <a href="#" class="waves-effect">{query41.Title}}<i class="material-icons">landscape</i></a>
                            </li>
                            <li>
                                <a href="#" class="waves-effect">{query42.Title}}<i class="material-icons">landscape</i></a>
                            </li>
                            <li>
                                <a href="#" class="waves-effect">{query43.Title}}<i class="material-icons">landscape</i></a>
                            </li>
                            <li>
                                <a href="#" class="waves-effect">{query44.Title}}<i class="material-icons">landscape</i></a>
                            </li>
                        </ul>
                    </div>
                </li>
                <li>
                    <a class="collapsible-header">{goup5.Title}}<i class="material-icons chevron">chevron_left</i></a>
                    <div class="collapsible-body">
                        <ul>
                            <li>
                                <a href="#" class="waves-effect">{query51.Title}}<i class="material-icons">landscape</i></a>
                            </li>
                            <li>
                                <a href="#" class="waves-effect">{query52.Title}}<i class="material-icons">landscape</i></a>
                            </li>
                            <li>
                                <a href="#" class="waves-effect">{query53.Title}}<i class="material-icons">landscape</i></a>
                            </li>
                            <li>
                                <a href="#" class="waves-effect">{query54.Title}}<i class="material-icons">landscape</i></a>
                            </li>
                        </ul>
                    </div>
                </li>
                <li>
                    <a class="collapsible-header">{goup6.Title}}<i class="material-icons chevron">chevron_left</i></a>
                    <div class="collapsible-body">
                        <ul>
                            <li>
                                <a href="#" class="waves-effect">{query61.Title}}<i class="material-icons">landscape</i></a>
                            </li>
                            <li>
                                <a href="#" class="waves-effect">{query62.Title}}<i class="material-icons">landscape</i></a>
                            </li>
                            <li>
                                <a href="#" class="waves-effect">{query63.Title}}<i class="material-icons">landscape</i></a>
                            </li>
                            <li>
                                <a href="#" class="waves-effect">{query64.Title}}<i class="material-icons">landscape</i></a>
                            </li>
                        </ul>
                    </div>
                </li>
            </ul>
        </li>
    </ul>
</template>
<script>
    export default {
        name: 'GroupsBar',
        props: {
            barId: String,
        },
        components: {
        },
        data() {
            return {
            }
        }
    }
</script>