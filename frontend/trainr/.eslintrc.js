module.exports = {
  extends: ["react-app", "react-app/jest"],
  rules: {
    // Enable unused variable detection
    "@typescript-eslint/no-unused-vars": [
      "error",
      {
        argsIgnorePattern: "^_",
        varsIgnorePattern: "^_",
        ignoreRestSiblings: true,
      },
    ],
    // Detect unused imports
    "no-unused-vars": "off", // Turn off base rule as it can report incorrect errors
    // Additional rules for detecting unused code
    "@typescript-eslint/no-unused-expressions": "error",
  },
  parser: "@typescript-eslint/parser",
  plugins: ["@typescript-eslint"],
};
