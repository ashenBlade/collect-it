import express from 'express';
import path from 'path';
const PORT = 5500;

const app = express();

app.get('/', (req, res) => {
    res.end('<h1>Hello, NodeJS + ExpressJS!</h1>')
})

app.listen(PORT, () => {
    console.log(`Server started on port ${PORT}`);
});
