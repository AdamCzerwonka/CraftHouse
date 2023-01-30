const { src, dest, watch } = require("gulp");
const sass = require("gulp-sass")(require("node-sass"));
const minifyCss = require("gulp-clean-css");
const sourcemaps = require("gulp-sourcemaps");

const bundleSass = () => {
  return src("./scss/**/*.scss")
    .pipe(sourcemaps.init())
    .pipe(sass().on("error", sass.logError))
    .pipe(minifyCss())
    .pipe(sourcemaps.write())
    .pipe(dest("./../CraftHouse.Web/wwwroot/css"));
};

const devWatch = () => {
  watch("./scss/**/*.scss", bundleSass);
};

exports.bundleSass = bundleSass;
exports.watch = devWatch;
