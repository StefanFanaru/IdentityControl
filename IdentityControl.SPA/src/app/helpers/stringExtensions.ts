declare global {
  interface String {
    firstCharUpper(): string;

    formatKey(): string;
  }
}

String.prototype.firstCharUpper = function () {
  let word = String(this);

  return word.charAt(0).toLocaleUpperCase() + word.substring(1, word.length);
};

String.prototype.formatKey = function () {
  let str = String(this);
  str = str.replace(/([a-z\xE0-\xFF])([A-Z\xC0\xDF])/g, '$1 $2');

  return str.toLocaleLowerCase().firstCharUpper();
};

// String.prototype.toPascalCase = function () {
//   const words = this.match(/[a-z]+/gi);
//   if (!words) {
//     return '';
//   }
//   return words
//     .map(function (word) {
//       return word.charAt(0).toUpperCase() + word.substr(1).toLowerCase();
//     })
//     .join('');
// };

export {};

export const toSnakeCase = str =>
  str &&
  str
    .match(/[A-Z]{2,}(?=[A-Z][a-z]+[0-9]*|\b)|[A-Z]?[a-z]+[0-9]*|[A-Z]|[0-9]+/g)
    .map(x => x.toLowerCase())
    .join('_');

export const toPascalCase = str =>
  str.replace(/\w+/g, w => w[0].toUpperCase() + w.slice(1).toLowerCase());
