<template>
    <div class="portal-query-workplace">

        <div class="row"></div>
        <div class="row">
            <div class="col s6">
                <ul class="collapsible">
                    <li>
                        <div class="collapsible-header"><i class="material-icons">collections</i><span><b>Group:</b> {currentGroup.Title}}</span></div>
                        <div class="collapsible-body"><span>{currentGroup.Description}}</span></div>
                    </li>
                    <li>
                        <div class="collapsible-header"><i class="material-icons">landscape</i><span><b>Query:</b> {currentQuery.Title}}</span></div>
                        <div class="collapsible-body"><span>{currentQuery.Description}}</span></div>
                    </li>
                </ul>
            </div>
        </div>

        <div class="card s12">

            <div class="card-action">
                <a class="waves-effect waves-light modal-trigger" href="#form-add-record">Add</a>

                <a href="#">Edit</a>
                <a href="#">Delete</a>
            </div>

            <div id="form-add-record" class="modal modal-fixed-footer">
                <div class="modal-content">
                    <h4>Add New Item: {currentQuery.Title}}</h4>
                    <div class="row portal-form-place">
                        <form class="col s12">
                            <div class="row">
                                <div class="input-field col s6">
                                    <i class="material-icons prefix">account_circle</i>
                                    <input id="icon_prefix" type="text" class="validate">
                                    <label for="icon_prefix">First Name</label>
                                </div>
                                <div class="input-field col s6">
                                    <i class="material-icons prefix">phone</i>
                                    <input id="icon_telephone" type="tel" class="validate">
                                    <label for="icon_telephone">Telephone</label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="input-field col s6">
                                    <input id="datetime" type="text" class="datepicker">
                                    <label for="datetime" class="active">Datetime</label>
                                </div>
                                <div class="input-field col s6">
                                    <input id="lunchtime" type="text" class="timepicker">
                                    <label for="lunchtime" class="active">Lunchtime</label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="input-field col s6">
                                    <input placeholder="Placeholder" id="first_name" type="text" class="validate">
                                    <label for="first_name">First Name</label>
                                </div>
                                <div class="input-field col s6">
                                    <input id="last_name" type="text" class="validate">
                                    <label for="last_name">Last Name</label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="input-field col s12">
                                    <input disabled value="I am not editable" id="disabled" type="text" class="validate">
                                    <label for="disabled">Disabled</label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="input-field col s12">
                                    <input id="password" type="password" class="validate">
                                    <label for="password">Password</label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="input-field col s12">
                                    <input id="email1" type="email" class="validate">
                                    <label for="email1">Email</label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col s12">
                                    This is an inline input field:
                                    <div class="input-field inline">
                                        <input id="email2" type="email" class="validate">
                                        <label for="email2" data-error="wrong" data-success="right">Email</label>
                                    </div>
                                </div>
                            </div>
                        </form>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="#!" class="modal-close waves-effect waves-green btn-flat">Close</a>
                    <a href="#!" class="modal-close waves-effect waves-green btn-flat">Save</a>
                </div>
            </div>

            <div class="card-content">
                <table class="bordered portal-data-table">
                    <thead>
                        <tr>
                            <th data-field="id">Name</th>
                            <th data-field="name">Item Name</th>
                            <th data-field="price">Item Price</th>
                            <th data-field="price">Item Price</th>
                            <th data-field="price">Item Price</th>
                            <th data-field="price">Item Price</th>
                            <th data-field="price">Item Price</th>
                            <th data-field="price">Item Price</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td>Alvin</td>
                            <td>Eclair</td>
                            <td>.87</td>

                            <td>.87</td>
                            <td>.87</td>
                            <td>.87</td>
                            <td>.87</td>
                            <td>.87</td>
                        </tr>
                        <tr>
                            <td>Alan</td>
                            <td>Jellybean</td>
                            <td>.76</td>
                            <td>.87</td>
                            <td>.87</td>
                            <td>.87</td>
                            <td>.87</td>
                            <td>.87</td>
                        </tr>
                        <tr>
                            <td>Jonathan</td>
                            <td>Lollipop</td>
                            <td>.00</td>
                            <td>.87</td>
                            <td>.87</td>
                            <td>.87</td>
                            <td>.87</td>
                            <td>.87</td>
                        </tr>
                        <tr>
                            <td>Jonathan</td>
                            <td>Lollipop</td>
                            <td>.00</td>
                            <td>.87</td>
                            <td>.87</td>
                            <td>.87</td>
                            <td>.87</td>
                            <td>.87</td>
                        </tr>
                        <tr>
                            <td>Jonathan</td>
                            <td>Lollipop</td>
                            <td>.00</td>
                            <td>.87</td>
                            <td>.87</td>
                            <td>.87</td>
                            <td>.87</td>
                            <td>.87</td>
                        </tr>
                        <tr>
                            <td>Jonathan</td>
                            <td>Lollipop</td>
                            <td>.00</td>
                            <td>.87</td>
                            <td>.87</td>
                            <td>.87</td>
                            <td>.87</td>
                            <td>.87</td>
                        </tr>
                        <tr>
                            <td>Jonathan</td>
                            <td>Lollipop</td>
                            <td>.00</td>
                            <td>.87</td>
                            <td>.87</td>
                            <td>.87</td>
                            <td>.87</td>
                            <td>.87</td>
                        </tr>
                        <tr>
                            <td>Jonathan</td>
                            <td>Lollipop</td>
                            <td>.00</td>
                            <td>.87</td>
                            <td>.87</td>
                            <td>.87</td>
                            <td>.87</td>
                            <td>.87</td>
                        </tr>
                        <tr>
                            <td>Jonathan</td>
                            <td>Lollipop</td>
                            <td>.00</td>
                            <td>.87</td>
                            <td>.87</td>
                            <td>.87</td>
                            <td>.87</td>
                            <td>.87</td>
                        </tr>
                        <tr>
                            <td>Jonathan</td>
                            <td>Lollipop</td>
                            <td>.00</td>
                            <td>.87</td>
                            <td>.87</td>
                            <td>.87</td>
                            <td>.87</td>
                            <td>.87</td>
                        </tr>
                        <tr>
                            <td>Jonathan</td>
                            <td>Lollipop</td>
                            <td>.00</td>
                            <td>.87</td>
                            <td>.87</td>
                            <td>.87</td>
                            <td>.87</td>
                            <td>.87</td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        name: 'QueryWorkplace',
        props: {
        },
        components: {
        },
        data() {
            return {
            }
        }
    }
</script>