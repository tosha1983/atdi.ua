var fpsSpan = document.getElementById("fps");
var xySpan = document.getElementById("xy");
var gridCanvas = document.getElementById("gridCanvas")
var mouseCanvas = document.getElementById("mouseCanvas")

var gridContext = gridCanvas.getContext("2d");
var mouseContext = mouseCanvas.getContext("2d");



var gridOptions = {
    width: gridCanvas.width,
    height: gridCanvas.height,
    xTickMinVal: 0,
    xTickMaxVal: 10,
    xTickSize: 1,
    xInnerTickCount: 5,
    

    yTickMinVal: -120,
    yTickMaxVal: -10,
    yTickSize: 10,
    yInnerTickCount: 5,

    showXGrid: true,
    showXInnerGrid: true,
    showVerticalGridlines: true,
    showVerticalInnerGridlines: true,

    showHorizontalGridlines: true,
    showHorizontalInnerGridlines: true,

    verticalGridlinesDush: [5,3],
    verticalInnerGridlinesDush: [2, 2],

    horizontalGridlinesDush: [5, 3],
    horizontalInnerGridlinesDush: [2, 2],

    innerLineThickness: 1,
    lineThickness: 1,

    stopped: false
};
var gridBuffer = document.createElement("canvas");
gridBuffer.width = gridOptions.width;
gridBuffer.height = gridOptions.height;

var gridBufferContext = gridContext; // gridBuffer.getContext("2d");

function updateGridBuffer(){
    
    gridBufferContext.clearRect(0, 0, gridOptions.width, gridOptions.height);

    

    // Create vertical gridlines:
    if (gridOptions.showVerticalGridlines){
        if (gridOptions.showVerticalInnerGridlines){
            
            gridBufferContext.beginPath();

            gridBufferContext.lineWidth = gridOptions.innerLineThickness;
            gridBufferContext.setLineDash(gridOptions.verticalInnerGridlinesDush);

            var step = gridOptions.xTickSize / gridOptions.xInnerTickCount
            for (var dx = gridOptions.xTickMinVal + step; dx < gridOptions.xTickMaxVal; dx += step){
                const fromPos = normalizeGridPoint(dx, gridOptions.yTickMinVal);
                const toPos = normalizeGridPoint(dx, gridOptions.yTickMaxVal);

                gridBufferContext.moveTo(fromPos.x, fromPos.y);
                gridBufferContext.lineTo(toPos.x, toPos.y);
            }

            gridBufferContext.stroke();
        }

        gridBufferContext.beginPath();

        gridBufferContext.lineWidth = gridOptions.lineThickness;
        gridBufferContext.setLineDash(gridOptions.verticalGridlinesDush);

        for (var dx = gridOptions.xTickMinVal + gridOptions.xTickSize; dx < gridOptions.xTickMaxVal; dx += gridOptions.xTickSize) {
            const fromPos = normalizeGridPoint(dx, gridOptions.yTickMinVal);
            const toPos = normalizeGridPoint(dx, gridOptions.yTickMaxVal);

            gridBufferContext.moveTo(fromPos.x, fromPos.y);
            gridBufferContext.lineTo(toPos.x, toPos.y);
        }

        gridBufferContext.stroke();
    }

    // Create horizontal gridlines:
    if (gridOptions.showHorizontalGridlines){
        if (gridOptions.showHorizontalInnerGridlines){
            
            gridBufferContext.beginPath();

            gridBufferContext.lineWidth = gridOptions.innerLineThickness;
            gridBufferContext.setLineDash(gridOptions.horizontalInnerGridlinesDush);

            var step = gridOptions.yTickSize / gridOptions.yInnerTickCount
            for (var dy = gridOptions.yTickMinVal + step; dy < gridOptions.yTickMaxVal; dy += step){
                const fromPos = normalizeGridPoint(gridOptions.xTickMinVal, dy);
                const toPos = normalizeGridPoint(gridOptions.xTickMaxVal, dy);

                gridBufferContext.moveTo(fromPos.x, fromPos.y);
                gridBufferContext.lineTo(toPos.x, toPos.y);
            }

            gridBufferContext.stroke();
        }

        gridBufferContext.beginPath();

        gridBufferContext.lineWidth = gridOptions.lineThickness;
        gridBufferContext.setLineDash(gridOptions.horizontalGridlinesDush);

        for (var dy = gridOptions.yTickMinVal + gridOptions.yTickSize; dy < gridOptions.yTickMaxVal; dy += gridOptions.yTickSize) {
            const fromPos = normalizeGridPoint(gridOptions.xTickMinVal, dy);
            const toPos = normalizeGridPoint(gridOptions.xTickMaxVal, dy);

            gridBufferContext.moveTo(fromPos.x, fromPos.y);
            gridBufferContext.lineTo(toPos.x, toPos.y);
        }

        gridBufferContext.stroke();
    }

    
}



function normalizeGridPoint(x, y) {
    return {
        x: ( (x - gridOptions.xTickMinVal) * gridOptions.width / (gridOptions.xTickMaxVal - gridOptions.xTickMinVal) ),
        y: ( gridOptions.height - (y - gridOptions.yTickMinVal) * gridOptions.height / (gridOptions.yTickMaxVal - gridOptions.yTickMinVal) )
    };
}

var mouseOptions = {
    width: mouseCanvas.width,
    height: mouseCanvas.height,
    mouseEntered: false,
    mousePosX: 0,
    mousePosY: 0,
    mouseRectPos: {x1: 0, x2: 0},
    mouseRected: false
};

var mouseBuffer = document.createElement("canvas");
mouseBuffer.width = mouseOptions.width;
mouseBuffer.height = mouseOptions.height;

var mouseBufferContext = mouseContext; // mouseBuffer.getContext("2d");



mouseBufferContext.strokeStyle = "blue";

mouseCanvas.onmousemove = function (event) {

    mouseOptions.mouseEntered = true;
    mouseOptions.mousePosX = event.offsetX;
    mouseOptions.mousePosY = event.offsetY;

    if (mouseOptions.mouseRected){
        mouseOptions.mouseRectPos.x2 = event.offsetX;

        xySpan.innerText = "X1: " + mouseOptions.mouseRectPos.x1 + "; X2: " + mouseOptions.mouseRectPos.x2
    }

    updateMouseBuffer();
};

mouseCanvas.onmouseleave = function (event) {
    mouseOptions.mouseEntered = false;
    updateMouseBuffer();
};

mouseCanvas.onmousedown = function( event ) {
    gridOptions.stopped = true;
    mouseOptions.mouseRected = true;
    mouseOptions.mouseRectPos.x1 = event.offsetX;
    mouseOptions.mouseRectPos.x2 = event.offsetX;

    xySpan.innerText = "X1: " + mouseOptions.mouseRectPos.x1 + "; X2: " + mouseOptions.mouseRectPos.x2

    updateMouseBuffer();
}

mouseCanvas.onmouseup = function(event){
    gridOptions.stopped = false;
    mouseOptions.mouseRected = false;
    mouseOptions.mouseRectPos.x2 = event.offsetX;

    xySpan.innerText = "X1: " + mouseOptions.mouseRectPos.x1 + "; X2: " + mouseOptions.mouseRectPos.x2 +"; Stopped";

    updateMouseBuffer();
}

mouseCanvas.onclick = function(event){
    //gridOptions.stopped = ! gridOptions.stopped;
}

function updateMouseBuffer(){
    mouseBufferContext.clearRect(0, 0, mouseOptions.width, mouseOptions.height);
    if (mouseOptions.mouseRected){
        mouseBufferContext.beginPath();

        mouseBufferContext.moveTo(mouseOptions.mouseRectPos.x1, 0);
        mouseBufferContext.lineTo(mouseOptions.mouseRectPos.x1, mouseOptions.height);

        mouseBufferContext.moveTo(mouseOptions.mouseRectPos.x2, 0);
        mouseBufferContext.lineTo(mouseOptions.mouseRectPos.x2, mouseOptions.height);

        if (Math.abs(mouseOptions.mouseRectPos.x2 - mouseOptions.mouseRectPos.x1) > 2){
            mouseBufferContext.fillStyle = "rgba(100,150,185,0.5)";
            mouseBufferContext.fillRect(mouseOptions.mouseRectPos.x1, 0, mouseOptions.mouseRectPos.x2 - mouseOptions.mouseRectPos.x1, mouseOptions.height);     
        }
        
        mouseBufferContext.stroke();
    }
    else if (mouseOptions.mouseEntered){
        mouseBufferContext.beginPath();

        mouseBufferContext.moveTo(0, mouseOptions.mousePosY);
        mouseBufferContext.lineTo(mouseOptions.width, mouseOptions.mousePosY);

        mouseBufferContext.moveTo(mouseOptions.mousePosX, 0);
        mouseBufferContext.lineTo(mouseOptions.mousePosX, mouseOptions.height);


        mouseBufferContext.stroke();
    }
}

function render() {
    mouseContext.clearRect(0, 0, mouseOptions.width, mouseOptions.height);
    mouseContext.drawImage(mouseBuffer, 0, 0);

    requestAnimationFrame(render);
}

var data = [];
function prepareData(count) {
    const step = (gridOptions.xTickMaxVal - gridOptions.xTickMinVal) / count
    for (let xVal = gridOptions.xTickMinVal; xVal < gridOptions.xTickMaxVal; xVal += step) {
        data.push({
            x: xVal,
            y: gridOptions.yTickMinVal + (Math.random() * (gridOptions.yTickMaxVal - gridOptions.yTickMinVal))
        });
    }

}

function renderGridLine(){
    gridBufferContext.lineWidth = 1;
    gridBufferContext.setLineDash([]);

    gridBufferContext.beginPath();
    var pos = normalizeGridPoint(data[0].x, data[0].y);
    gridBufferContext.moveTo(pos.x, pos.y);
    for (let index = 1; index < data.length; index++) {
        pos = normalizeGridPoint(data[index].x, data[index].y);
        gridBufferContext.lineTo(pos.x, pos.y);
    }
    gridBufferContext.stroke();
}
var dataCount = 4000;

prepareData(dataCount);
updateGridBuffer();
renderGridLine();

var fpsCount = 0 ;
var now = new Date();
var currentSecond = now.getSeconds();

setInterval(function (){
    if (gridOptions.stopped){
        return;
    }

    data = [];
    prepareData(dataCount);
    updateGridBuffer();
    renderGridLine();

    let now2 = new Date();
    let currentSecond2 = now2.getSeconds();

    if (currentSecond === currentSecond2){
        ++ fpsCount;
    } else {
        console.log("FPS [" + currentSecond + "]: " + fpsCount);
        fpsSpan.innerText = fpsCount;
        fpsCount = 1;
        currentSecond = currentSecond2

        
    }
}, 1);


//render();