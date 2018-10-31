document.addEventListener('DOMContentLoaded',
    function initMaterialize() {

        const sidenavElements = document.querySelectorAll('.sidenav');
        const dropdownTriggerElements = document.querySelectorAll('.dropdown-trigger');
        const tooltipElements = document.querySelectorAll('.tooltipped');
        const collapsibleElements = document.querySelectorAll('.collapsible');
        const modalElements = document.querySelectorAll('.modal');
        const datepickerElements = document.querySelectorAll('.datepicker');
        const timepickerElements = document.querySelectorAll('.timepicker');

        M.Sidenav.init(sidenavElements);
        M.Dropdown.init(dropdownTriggerElements);
        M.Tooltip.init(tooltipElements);
        M.Collapsible.init(collapsibleElements);
        M.Modal.init(modalElements);

        const datepickerOptions = {
            container: document.getElementById('hidden-date')
        };
        M.Datepicker.init(datepickerElements, datepickerOptions);

        const timepickerOptions = {
            container: '#hidden-time'
        };
        M.Timepicker.init(timepickerElements, timepickerOptions);

    }
);