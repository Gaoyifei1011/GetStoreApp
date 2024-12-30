using GetStoreAppInstaller.Extensions.DataType.Methods;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GetStoreAppInstaller.Extensions.PriExtract
{
    public sealed class ReferencedFileSection
    {
        public uint SectionQualifier { get; private set; }

        public uint Flags { get; private set; }

        public uint SectionFlags { get; private set; }

        public uint SectionLength { get; private set; }

        public IReadOnlyList<ReferencedFileOrFolder> ReferencedFilesList { get; private set; }

        public ReferencedFileSection(string sectionIdentifier, BinaryReader binaryReader)
        {
            if (new string(binaryReader.ReadChars(16)) != sectionIdentifier)
            {
                throw new InvalidDataException("Unexpected section identifier.");
            }

            SectionQualifier = binaryReader.ReadUInt32();
            Flags = binaryReader.ReadUInt16();
            SectionFlags = binaryReader.ReadUInt16();
            SectionLength = binaryReader.ReadUInt32();
            binaryReader.ExpectUInt32(0);

            binaryReader.BaseStream.Seek(SectionLength - 16 - 24, SeekOrigin.Current);

            binaryReader.ExpectUInt32(0xDEF5FADE);
            binaryReader.ExpectUInt32(SectionLength);

            binaryReader.BaseStream.Seek(-8 - (SectionLength - 16 - 24), SeekOrigin.Current);

            using SubStream subStream = new(binaryReader.BaseStream, binaryReader.BaseStream.Position, (int)SectionLength - 16 - 24);
            using BinaryReader subBinaryReader = new(subStream, Encoding.ASCII);

            ushort numRoots = binaryReader.ReadUInt16();
            ushort numFolders = binaryReader.ReadUInt16();
            ushort numFiles = binaryReader.ReadUInt16();
            binaryReader.ExpectUInt16(0);
            uint totalDataLength = binaryReader.ReadUInt32();

            List<FolderInfo> folderInfosList = new(numFolders);

            for (int i = 0; i < numFolders; i++)
            {
                binaryReader.ExpectUInt16(0);
                ushort parentFolder = binaryReader.ReadUInt16();
                ushort numFoldersInFolder = binaryReader.ReadUInt16();
                ushort firstFolderInFolder = binaryReader.ReadUInt16();
                ushort numFilesInFolder = binaryReader.ReadUInt16();
                ushort firstFileInFolder = binaryReader.ReadUInt16();
                ushort folderNameLength = binaryReader.ReadUInt16();
                ushort fullPathLength = binaryReader.ReadUInt16();
                uint folderNameOffset = binaryReader.ReadUInt32();
                folderInfosList.Add(new FolderInfo()
                {
                    ParentFolder = parentFolder,
                    NumFilesInFolder = numFilesInFolder,
                    FirstFileInFolder = firstFileInFolder,
                    NumFoldersInFolder = numFoldersInFolder,
                    FirstFolderInFolder = firstFolderInFolder,
                    FolderNameLength = folderNameLength,
                    FullPathLength = fullPathLength,
                    FolderNameOffset = folderNameOffset,
                });
            }

            List<FileInfo> fileInfos = new(numFiles);

            for (int i = 0; i < numFiles; i++)
            {
                binaryReader.ReadUInt16();
                ushort parentFolder = binaryReader.ReadUInt16();
                ushort fullPathLength = binaryReader.ReadUInt16();
                ushort fileNameLength = binaryReader.ReadUInt16();
                uint fileNameOffset = binaryReader.ReadUInt32();
                fileInfos.Add(new FileInfo()
                {
                    ParentFolder = parentFolder,
                    FullPathLength = fileNameLength,
                    FileNameLength = fileNameLength,
                    FileNameOffset = fileNameOffset,
                });
            }

            long dataStartPosition = binaryReader.BaseStream.Position;

            List<ReferencedFileOrFolder> referencedFolders = new(numFolders);

            for (int i = 0; i < numFolders; i++)
            {
                binaryReader.BaseStream.Seek(dataStartPosition + folderInfosList[i].FolderNameOffset * 2, SeekOrigin.Begin);

                string name = binaryReader.ReadString(Encoding.Unicode, folderInfosList[i].FolderNameLength);

                referencedFolders.Add(new ReferencedFileOrFolder()
                {
                    Parent = null,
                    Name = name,
                    Children = null
                });
            }

            for (int i = 0; i < numFolders; i++)
            {
                if (folderInfosList[i].ParentFolder is not 0xFFFF)
                {
                    referencedFolders[i].Parent = referencedFolders[folderInfosList[i].ParentFolder];
                }
            }

            List<ReferencedFileOrFolder> referencedFilesList = new(numFiles);

            for (int i = 0; i < numFiles; i++)
            {
                binaryReader.BaseStream.Seek(dataStartPosition + fileInfos[i].FileNameOffset * 2, SeekOrigin.Begin);

                string name = binaryReader.ReadString(Encoding.Unicode, fileInfos[i].FileNameLength);

                ReferencedFileOrFolder parentFolder = fileInfos[i].ParentFolder is not 0xFFFF ? referencedFolders[fileInfos[i].ParentFolder] : null;
                referencedFilesList.Add(new ReferencedFileOrFolder()
                {
                    Parent = parentFolder,
                    Name = name,
                });
            }

            for (int i = 0; i < numFolders; i++)
            {
                List<ReferencedFileOrFolder> children = new(folderInfosList[i].NumFoldersInFolder + folderInfosList[i].NumFilesInFolder);

                for (int j = 0; j < folderInfosList[i].NumFoldersInFolder; j++)
                {
                    children.Add(referencedFolders[folderInfosList[i].FirstFolderInFolder + j]);
                }

                for (int j = 0; j < folderInfosList[i].NumFilesInFolder; j++)
                {
                    children.Add(referencedFilesList[folderInfosList[i].FirstFileInFolder + j]);
                }

                referencedFolders[i].Children = children;
            }

            ReferencedFilesList = referencedFilesList;
        }
    }
}
