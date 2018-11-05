<template>
    <nav class="portal-mainbar portal-content-header">
        <div class="nav-wrapper">
            <a href="#!" :data-target="barId" class="sidenav-trigger left"><i class="material-icons green-text">menu</i></a>

            <ul class="brand-logo" style="padding-left: 10px">
                <li class="valign-wrapper">
                    <span style="font-size: 1.5rem;"><b>WebQuery Portal</b></span>
                </li>

            </ul>
            <ul class="right hide-on-med-and-down">
                <li class="valign-wrapper">
                    <span>
                        {{userName}}
                        <i class="material-icons left">person</i>
                    </span>
                </li>
                <li>
                    <a class="waves-effect tooltipped" data-position="bottom" v-bind:data-tooltip="'Sign Out ' + userName" :href="signOutAction">
                        <i class="material-icons">power_settings_new</i>
                    </a>
                </li>
            </ul>

            <a href="#!" data-target="right-dropdown-content" class="sidenav-trigger dropdown-trigger right"><i class="material-icons green-text">filter_list</i></a>
            <ul id="right-dropdown-content" class="dropdown-content" style="min-width: 250px;">
                <li>
                    <a class="waves-effect active" :href="signOutAction">
                        Sign Out {{userName}}
                        <i class="material-icons">power_settings_new</i>
                    </a>
                </li>
            </ul>

        </div>
    </nav>
</template>

<script>
    export default {
        name: 'MainBar',
        props: {
            barId: String,
        },
        components: {
        },
        data() {
            return {
                userName: 'AndreyKovpak',
                signOutAction: 'Account/SignOut'
            }
        }
    }
</script>
