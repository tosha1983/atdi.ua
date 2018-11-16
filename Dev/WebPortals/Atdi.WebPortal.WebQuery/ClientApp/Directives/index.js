const directives = {
    install($vue) {
        $vue.directive('visible', (el, binding) => {
            var value = binding.value;

            if (!!value) {
                el.style.visibility = 'visible';
            } else {
                el.style.visibility = 'hidden';
            }
        });
    }
};

export default directives;