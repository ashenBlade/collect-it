import express from 'express';
import path from 'path';
const PORT = 5500;

const app = express();

app.listen(PORT, () => {
    console.log(`Server started on port ${PORT}`);
});
