<template>
    <footer class="page-footer portal-footer z-depth-4">
        <div class="footer-copyright">
            <div class="container">
                Developed by L.I.S Ukraine © {{(new Date()).getFullYear()}}
                <a class="grey-text text-lighten-4 right" :href="'//' + site" target="_blank">{{title}}</a>
            </div>
        </div>
    </footer>
</template>

<script>
    export default {
        name: 'FooterBar',
        props: {
            barId: String,
            title: String,
            site: String
        },
        components: {
        },
        data() {
            return {
            }
        }
    }
</script>