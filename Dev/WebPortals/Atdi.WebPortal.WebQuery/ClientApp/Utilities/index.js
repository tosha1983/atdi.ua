const SEARCHABLE_TYPES = ["string", "number", "boolean"];

export default {

    applyFilter: function (source, filter) {
        console.log("[call: applyFilter] - value: "+ filter);
        if (!filter) {
            return source;
        }

        let filtered = [];
        let searchValue  = filter.toLowerCase()

        for (let i = 0; i < source.length; i++) {
            let item = source[i];

            for (let prop in item) {
                let value = item[prop];

                // Ensure the value is of a searchable type 
                // This will automatically handle null values
                if (SEARCHABLE_TYPES.indexOf(typeof (value)) < 0) {
                    continue;
                }

                // Normalise the value to get a consistent match
                let normalised = value.toString().toLowerCase();

                if (normalised.indexOf(searchValue) > -1) {
                    filtered.push(item);
                    break;
                }
            }
        }

        return filtered;
    },

    applySorting: function (source, callback, direction) {

        console.log("[call: applySorting] - direction: "+ direction);

        direction = direction || 1;
        callback = callback || (item => item);

        if (Math.abs(direction) !== 1) {
            throw new Error("Sort direction must be either 1 (ascending) or -1 (descending)");
        }

        let sortArray = [...source];

        sortArray.sort((a, b) => {
            let valueA = callback.call(source, a);
            let valueB = callback.call(source, b);

            if (valueA === null) {
                valueA = ''
            }
            if (valueB === null) {
                valueB = ''
            }
            let outcome = valueA < valueB ? -1 : valueA > valueB ? 1 : 0;

            return outcome * direction;
        });

        return sortArray;
    },

    applyMultiSorting: function (source, columns) {

        console.log("[call: applyMultiSorting] - columns: ", columns);

        let sortArray = [...source];
        sortArray.sort((row1, row2) => {

            for (let index = 0; index < columns.length; index++) {
                const column = columns[index];
                
                let valueRow1 = row1[column.index]; // callback.call(source, row1, column);
                let valueRow2 = row2[column.index]; //callback.call(source, row2, column);

                if (valueRow1 === null) {
                    valueRow1 = ''
                }
                if (valueRow2 === null) {
                    valueRow2 = ''
                }

                if (valueRow1 < valueRow2) {
                    return -1 * column.direction;
                }
                if (valueRow1 > valueRow2) {
                    return 1 * column.direction;
                }
            }

            return 0;
        });

        return sortArray;
    }
}