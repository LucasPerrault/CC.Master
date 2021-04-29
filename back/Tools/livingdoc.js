const { promisify } = require('util');
const { resolve } = require('path');
const fs = require('fs');
const readdir = promisify(fs.readdir);
const stat = promisify(fs.stat);

const baseDir = "back\\";

async function getFiles(dir) {
  const subdirs = await readdir(dir);
  const files = await Promise.all(subdirs.map(async (subdir) => {
    const res = resolve(dir, subdir);
    return (await stat(res)).isDirectory() ? getFiles(res) : res;
  }));
  return files.reduce((a, f) => a.concat(f), []);
}

function mergeFeatureDataFiles(files) {
    let mergedData = null;
    for (index in files){
        const data = fs.readFileSync(files[index], 'utf8');
        const featureData = JSON.parse(data);
        const regexp = new RegExp(`.*${baseDir.replace(/\\/, '\\\\')}`);
        const title = files[index].replace(regexp, '').replace(/bin.*$/, '').replace(/\\/g, '/');
        console.log(title);
        featureData.ExecutionResults = featureData.ExecutionResults.map(er => ({...er, FeatureFolderPath: title + er.FeatureFolderPath}));
        if (mergedData == null)
        {
            mergedData = featureData;
        }
        else {
            mergedData.ExecutionResults.push(...featureData.ExecutionResults); 
        }
    }
    fs.writeFileSync("TestExecution.json", JSON.stringify(mergedData));
}

getFiles(baseDir)
  .then(files => files.filter(f => f.indexOf("TestExecution.json") != -1))
  .then(files => mergeFeatureDataFiles(files))
  .catch(e => console.error(e));