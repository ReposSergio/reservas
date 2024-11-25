import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  /* config options here */
};

// next.config.js
module.exports = {
  eslint: {
    ignoreDuringBuilds: true,  // Desactiva ESLint en el proceso de construcción
  },
}


export default nextConfig;
